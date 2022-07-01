using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using GDPR_Download.API;
using GDPR_Download.API.Data;
using GDPR_Download.API.Models;
using GDPR_Download.Common;
using GDPR_Download.Helpers;
using GDPR_Download.Models;
using Ionic.Zip;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace GDPR_Download.Controllers
{
    [RoutePrefix("api/Upload")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class UploadController : ApiController
    {
        static string StoragePath = ConfigurationManager.AppSettings["storagepath"];

        [HttpPost]
        [Route("Files/{password}")]
        public async Task<HttpResponseMessage> UploadFIles(string password)
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                if (!Directory.Exists(Path.Combine(StoragePath, "tmp")))
                {
                    Directory.CreateDirectory(Path.Combine(StoragePath, "tmp"));
                }

                var streamProvider = new MultipartFormDataStreamProvider(Path.Combine(StoragePath, "tmp"));
                await Request.Content.ReadAsMultipartAsync(streamProvider);
                Dictionary<string, string> Files = new Dictionary<string, string>();

                foreach (MultipartFileData fileData in streamProvider.FileData)
                {
                    if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                    {
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
                    }
                    string fileName = fileData.Headers.ContentDisposition.FileName;
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Trim('"');
                    }
                    if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                    {
                        fileName = Path.GetFileName(fileName);
                    }
                    Files.Add(fileData.LocalFileName, fileName);
                }

                string zipName = Guid.NewGuid() + ".zip";
                string zip = StoragePath + zipName;
                using (Ionic.Zip.ZipFile passwordZIP = new Ionic.Zip.ZipFile())
                {
                    passwordZIP.Password = password;
                    foreach (var item in Files)
                    {
                        passwordZIP.AddFile(item.Key, "").FileName = item.Value;
                    }
                    passwordZIP.Save(zip);
                }


                foreach (var item in Files)
                {
                    if (File.Exists(item.Key))
                        File.Delete(item.Key);
                }

                return Request.CreateResponse(HttpStatusCode.OK, zipName);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("download/{name}")]
        public HttpResponseMessage ZipDocs(string name)
        {
            using (SiteDbContext dbContext = new SiteDbContext())
            {
                FileModel file = dbContext.Files.Where(x => x.FileName == name + ".zip").FirstOrDefault();
                if (file == null)
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                file.Opened = true;
                file.OpenedDate = DateTime.Now;
                dbContext.Files.AddOrUpdate(file);
                dbContext.SaveChanges();
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.AddFile(StoragePath + name + ".zip", @"\");
                return ZipContentResult(zip);
            }
        }

        protected HttpResponseMessage ZipContentResult(ZipFile zipFile)
        {
            var pushStreamContent = new PushStreamContent((stream, content, context) =>
            {
                zipFile.Save(stream);
                stream.Close();
            }, "application/zip");

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = pushStreamContent };
        }

        [HttpPost]
        [Route("sendmail")]
        public async Task<IHttpActionResult> SendMailAsync([FromBody]string Email)
        {
            var id = HttpContext.Current.User.Identity.GetUserId();
            if (id == null)
            {
                return Unauthorized();
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(id);
            string senderEmail = user.Email;
            SMTP smtp = new SMTP();
            try
            {
                //string[] urlInfo = HttpContext.Current.Request.Url.AbsoluteUri.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                //bool ssl = false;
                //int test = 0;
                //for (int i = 0; i < urlInfo.Count(); i++)
                //{
                //    if (urlInfo[i] == "https:")
                //    {
                //        ssl = true;
                //    }

                //    if (urlInfo[i] == "Upload")
                //    {
                //        test = i - 2;
                //    }
                //}

                //string url = "";
                //if (ssl == true)
                //{
                //    url = "https://";
                //}
                //else
                //{
                //    url = "http://";
                //}

                //for (int i = 0; i <= test; i++)
                //{
                //    if (i != 0)
                //    {
                //        url += urlInfo[i] + "/";
                //    }
                //}

                string loginToken = Guid.NewGuid().ToString();

                var url = HttpContext.Current.Request.UrlReferrer.AbsoluteUri + "#!/home?loginToken=";
                var sent = smtp.SendLoginMail(Email, senderEmail, url, loginToken);
                if (sent == true)
                {
                    using (SiteDbContext dbContext = new SiteDbContext())
                    {
                        dbContext.Guests.AddOrUpdate(new GuestLoginModel
                        {
                            Mail = Email,
                            Sent = false,
                            UserId = User.Identity.GetUserId().ToString(),
                            LoginToken = Guid.Parse(loginToken),
                            CreatedDate = DateTime.Now
                        });
                        dbContext.SaveChanges();
                    }
                }
                return Ok(sent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            return Ok();
        }

        [HttpPost]
        [Route("send")]
        public async Task<IHttpActionResult> SendFileAsync(UploadInfoDOT uploadInfo)
        {
            var isGuest = HttpContext.Current.User.IsInRole("Gäst");
            var id = HttpContext.Current.User.Identity.GetUserId();
            if (id == null && !isGuest)
            {
                return Unauthorized();
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(id);
            string senderEmail = "";
            if (user != null)
                senderEmail = user.Email;
            else
                senderEmail = HttpContext.Current.User.Identity.Name;

            GuestLoginModel guest = new GuestLoginModel();

            if (isGuest)
            {
                using (SiteDbContext dbContext = new SiteDbContext())
                {
                    var identity = (System.Security.Claims.ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;

                    string loginToken = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Thumbprint)
                                       .Select(c => c.Value).SingleOrDefault();
                    Guid gLoginToken = Guid.Parse(loginToken);
                    guest = dbContext.Guests.Where(x => x.LoginToken == gLoginToken).FirstOrDefault();
                    ApplicationUser mainUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(guest.UserId);
                    uploadInfo.ReciverEmail = mainUser.Email;
                }
            }

            SMTP smtp = new SMTP();
            try
            {
                string[] urlInfo = HttpContext.Current.Request.Url.AbsoluteUri.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                bool ssl = false;
                int test = 0;
                for (int i = 0; i < urlInfo.Count(); i++)
                {
                    if (urlInfo[i] == "https:")
                    {
                        ssl = true;
                    }

                    if (urlInfo[i] == "Upload")
                    {
                        test = i - 2;
                    }
                }

                string url = "";
                if (ssl == true)
                {
                    url = "https://";
                }
                else
                {
                    url = "http://";
                }

                for (int i = 0; i <= test; i++)
                {
                    if (i != 0)
                    {
                        url += urlInfo[i] + "/";
                    }
                }

                var sent = smtp.SendMail(uploadInfo, senderEmail, url);
                if (sent == true)
                {
                    using (SiteDbContext dbContext = new SiteDbContext())
                    {
                        dbContext.Files.AddOrUpdate(new FileModel
                        {
                            Description = uploadInfo.Description,
                            Subject = uploadInfo.Description,
                            FileName = uploadInfo.FileName,
                            UploadDate = DateTime.Now,
                            Opened = false,
                            OpenedDate = null
                        });
                        if (isGuest)
                        {
                            dbContext.Guests.Attach(guest);
                            dbContext.Guests.Remove(guest);
                        }
                        dbContext.SaveChanges();
                    }
                }
                return Ok(sent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
