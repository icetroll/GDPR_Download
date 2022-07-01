using System;
using System.Collections.Generic;
using System.Configuration;
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
using GDPR_Download.Helpers;
using GDPR_Download.Models;
using Ionic.Zip;

namespace GDPR_Download.Controllers
{
    [RoutePrefix("api/Upload")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UploadController : ApiController
    {
        static string StoragePath = ConfigurationManager.AppSettings["StoragePath"];
        
        [HttpPost]
        [Route("Files")]
        public async Task<HttpResponseMessage> UploadFIles()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                if(!Directory.Exists(Path.Combine(StoragePath, "tmp")))
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
                    passwordZIP.Password = "Test123!";
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

        public UploadInfoResponseDOT SendFile(UploadInfoDOT uploadInfo)
        {
            UploadInfoResponseDOT uploadInfoResponse = new UploadInfoResponseDOT();

            SMTP smtp = new SMTP();
            var sent = smtp.SendMail(uploadInfo);
            uploadInfo.Sent = sent;

            uploadInfoResponse.uploadInfoDOTs.Add(uploadInfo);
            return uploadInfoResponse;
        }
    }
}
