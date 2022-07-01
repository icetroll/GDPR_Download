using GDPR_Download.API.Models;
using GDPR_Download.Helpers;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml;

namespace GDPR_Download.API.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        private ApplicationDbContext usersContext = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public class Settings
        {
            public SQLSettings Sql { get; set; }
            public SMTPSettings Smtp { get; set; }
        }

        public class SQLSettings
        {
            public string Host { get; set; } = "";
            public string Port { get; set; } = "";
            public string Database { get; set; } = "";
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public class SMTPSettings
        {
            public string Host { get; set; } = "";
            public string Port { get; set; } = "";
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public class UpdateSettings
        {
            public string smtpHost { get; set; }
            public string smtpPassword { get; set; }
            public string smtpPort { get; set; }
            public string smtpUsername { get; set; }
            public string sqlDatabase { get; set; }
            public string sqlHost { get; set; }
            public string sqlPassword { get; set; }
            public string sqlPort { get; set; }
            public string sqlUsername { get; set; }

        }

        private void AddUpdateConnectionString(string newConString, UpdateSettings settingsInfo)
        {
            bool isNew = false;
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Web.Config");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList list = doc.DocumentElement.SelectNodes("connectionStrings/add[@name='DefaultConnection']");
            XmlNode smtpNode = doc.DocumentElement.SelectSingleNode("//add[@key='smtp']");
            XmlElement value = (XmlElement)smtpNode.SelectSingleNode("//add[@key='smtp']");
            //smtpNode.Attribute["value"].Value

            string newSmtpConnection = "Host=" + settingsInfo.smtpHost;
            if (settingsInfo.smtpPort != "")
            {
                newSmtpConnection += ";Port=" + settingsInfo.smtpPort;
            }
            if (settingsInfo.smtpUsername != "")
            {
                newSmtpConnection += ";Username=" + settingsInfo.smtpUsername;
            }
            if (settingsInfo.smtpPassword != "")
            {
                newSmtpConnection += ";Password=" + settingsInfo.smtpPassword;
            }


            smtpNode.Attributes["value"].Value = newSmtpConnection;

            XmlNode node;
            isNew = list.Count == 0;
            if (isNew)
            {
                node = doc.CreateNode(XmlNodeType.Element, "add", null);
                XmlAttribute attribute = doc.CreateAttribute("name");
                attribute.Value = "DefaultConnection";
                node.Attributes.Append(attribute);

                attribute = doc.CreateAttribute("connectionString");
                attribute.Value = "";
                node.Attributes.Append(attribute);

                attribute = doc.CreateAttribute("providerName");
                attribute.Value = "System.Data.SqlClient";
                node.Attributes.Append(attribute);
            }
            else
            {
                node = list[0];
            }
            string conString = node.Attributes["connectionString"].Value;
            SqlConnectionStringBuilder conStringBuilder = new SqlConnectionStringBuilder(newConString);
            if (settingsInfo.sqlPort != "")
            {
                conStringBuilder.DataSource = settingsInfo.sqlDatabase + ":" + settingsInfo.sqlPort;
            }
            else
            {
                conStringBuilder.DataSource = settingsInfo.sqlHost;
            }
            conStringBuilder.InitialCatalog = settingsInfo.sqlDatabase;
            conStringBuilder.IntegratedSecurity = true;
            if (settingsInfo.sqlUsername != "")
            {
                conStringBuilder.UserID = settingsInfo.sqlUsername;
            }
            if (settingsInfo.sqlPassword != "")
            {
                conStringBuilder.Password = settingsInfo.sqlPassword;
            }
            node.Attributes["connectionString"].Value = conStringBuilder.ConnectionString;
            if (isNew)
            {
                doc.DocumentElement.SelectNodes("connectionStrings")[0].AppendChild(node);
            }
            doc.Save(path);
        }

        private void UpdateSqlInfo(string connectionString)
        {
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString = connectionString;
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        [HttpPost]
        [Route("setSettings")]
        public IHttpActionResult SetSettings(UpdateSettings settingsInfo)
        {
            string sqlString = "";
            if (settingsInfo.sqlPort != "")
            {
                sqlString = "Server=" + settingsInfo.sqlHost + ":" + settingsInfo.sqlPort + ";Database=" + settingsInfo.sqlDatabase + ";Integrated Security=True;";
            }
            else
            {
                sqlString = "Server=" + settingsInfo.sqlHost + ";Database=" + settingsInfo.sqlDatabase + ";Integrated Security=True;";
            }

            if (settingsInfo.sqlUsername != "")
            {
                sqlString += "User id=" + settingsInfo.sqlUsername;
            }
            if (settingsInfo.sqlPassword != "")
            {
                sqlString += ";Password=" + settingsInfo.sqlPassword;
            }
            AddUpdateConnectionString(sqlString, settingsInfo);
            return Ok();
        }

        [HttpGet]
        [Route("getSettings")]
        public IHttpActionResult GetSettings()
        {
            Settings responseSettings = new Settings();
            SQLSettings sqlSetting = new SQLSettings();
            string sql = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string[] sqlInfos = sql.Split(';');
            foreach (string sqlInfo in sqlInfos)
            {
                if (sqlInfo == "")
                {

                }
                else
                {
                    string sqlInfoName = sqlInfo.Split('=')[0];
                    string sqlInfoValue = sqlInfo.Split('=')[1];
                    if (sqlInfoName == "Server" || sqlInfoName == "Data Source")
                    {
                        if (sqlInfoValue.Contains(":"))
                        {
                            sqlSetting.Host = sqlInfoValue.Split(':')[0];
                            sqlSetting.Port = sqlInfoValue.Split(':')[1];
                        }
                        else
                        {
                            sqlSetting.Host = sqlInfoValue;
                        }
                    }
                    if (sqlInfoName == "Database" || sqlInfoName == "Initial Catalog")
                    {
                        sqlSetting.Database = sqlInfoValue;
                    }
                    if (sqlInfoName == "User id" || sqlInfoName == "User ID")
                    {
                        sqlSetting.Username = sqlInfoValue;
                    }
                    if (sqlInfoName == "Password")
                    {
                        sqlSetting.Password = sqlInfoValue;
                    }
                }
            }

            SMTPSettings smtpSetting = new SMTPSettings();
            string smtp = ConfigurationManager.AppSettings["smtp"];
            string[] smtpInfos = smtp.Split(';');
            foreach (string smtpInfo in smtpInfos)
            {
                if (smtpInfo == "")
                {

                }
                else
                {
                    string smtpInfoName = smtpInfo.Split('=')[0];
                    string smtpInfoValue = smtpInfo.Split('=')[1];

                    if (smtpInfoName == "Host")
                    {
                        smtpSetting.Host = smtpInfoValue;
                    }
                    if (smtpInfoName == "Port")
                    {
                        smtpSetting.Port = smtpInfoValue;
                    }
                    if (smtpInfoName == "Username")
                    {
                        smtpSetting.Username = smtpInfoValue;
                    }
                    if (smtpInfoName == "Password")
                    {
                        smtpSetting.Password = smtpInfoValue;
                    }
                }
            }

            responseSettings.Sql = sqlSetting;
            responseSettings.Smtp = smtpSetting;
            return Ok(responseSettings);
        }

        [HttpGet]
        [Route("getallUsers/{pageSize:int}/{pageNumber:int}")]
        public IHttpActionResult getallUsers(int pageSize, int pageNumber)
        {
            UserResponseModel response = new UserResponseModel();

            List<ApplicationUser> users = usersContext.Users.ToList();
            List<UserModel> responseUsers = new List<UserModel>();
            foreach (var user in users)
            {
                string rolesResponse = "";
                foreach (var role in user.Roles)
                {
                    if (rolesResponse == "")
                    {
                        if (usersContext.Roles.Where(x => x.Id == role.RoleId).FirstOrDefault().Name == "Admin")
                        {
                            rolesResponse += "Administratör";
                        }
                        else if(usersContext.Roles.Where(x => x.Id == role.RoleId).FirstOrDefault().Name == "User")
                        {
                            rolesResponse += "Användare";
                        }
                        else
                        {
                            rolesResponse += usersContext.Roles.Where(x => x.Id == role.RoleId).FirstOrDefault().Name;
                        }
                    }
                    else
                    {
                        if (usersContext.Roles.Where(x => x.Id == role.RoleId).FirstOrDefault().Name == "Admin")
                        {
                            rolesResponse += ", Administratör";
                        }
                        else if (usersContext.Roles.Where(x => x.Id == role.RoleId).FirstOrDefault().Name == "User")
                        {
                            rolesResponse += ", Användare";
                        }
                        else
                        {
                            rolesResponse += ", " + usersContext.Roles.Where(x => x.Id == role.RoleId).FirstOrDefault().Name;
                        }
                    }
                }

                UserModel responseUser = new UserModel
                {
                    Email = user.Email,
                    Name = user.Name,
                    Roles = rolesResponse
                };
                responseUsers.Add(responseUser);
            }

            response.Users = responseUsers;
            response.totalCount = 0;
            response.PageNumber = pageNumber;

            return Ok(response);
        }

        [HttpPost]
        [Route("RemoveUser")]
        public async Task<IHttpActionResult> RemoveUserAsync(string email)
        {
            try
            {
                ApplicationUser user = UserManager.Users.Where(x => x.Email == email).FirstOrDefault();
                await UserManager.DeleteAsync(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("updateUser")]
        public async System.Threading.Tasks.Task<IHttpActionResult> UpdateUserAsync(UpdateUser userInfo)
        {
            ApplicationUser user = UserManager.Users.Where(x => x.UserName == userInfo.oldEmail).FirstOrDefault();
            if (user != null)
            {
                int amountUsers = UserManager.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains("2")).Count();

                if (userInfo.Admin == false && userInfo.User == false)
                {
                    return BadRequest("Du måste välja en roll");
                }
                else if (userInfo.Admin == false && userInfo.User == true && amountUsers > LicenseSystem.usersAllowed)
                {
                    return BadRequest("Du måste höja användar antalet på din licens");
                }

                    user.UserName = userInfo.Email;
                user.Email = userInfo.Email;
                user.Name = userInfo.Name;

                await UserManager.UpdateAsync(user);
                if (userInfo.Password != "")
                {
                    // Remove old password
                    await UserManager.RemovePasswordAsync(user.Id);
                    // Set the new password
                    await UserManager.AddPasswordAsync(user.Id, userInfo.Password);
                }

                if (userInfo.Admin == true)
                {
                    await UserManager.AddToRoleAsync(user.Id, "Administratör");
                }
                else
                {
                    await UserManager.RemoveFromRoleAsync(user.Id, "Administratör");
                }


                // Set customer amount
                if (userInfo.User == true && amountUsers < LicenseSystem.usersAllowed)
                {
                    await UserManager.AddToRoleAsync(user.Id, "Användare");
                }

                if (userInfo.User == false)
                {
                    await UserManager.RemoveFromRoleAsync(user.Id, "Användare");
                }

                UserManager.Dispose();

                if (userInfo.Admin == true && userInfo.User == true && amountUsers > LicenseSystem.usersAllowed)
                {
                    return BadRequest("Användaren har lagts till som admin men inte som användare." +
                        "\r\n Du måste höja användar antalet på din licens");
                }
                else if (userInfo.User == true && amountUsers > LicenseSystem.usersAllowed)
                {
                    return BadRequest("Du måste höja användar antalet på din licens");
                }
                else
                {
                    return Ok();
                }
            }
            else
            {
                return BadRequest("Användaren kunde inte hittas");
            }
        }
    }
}
