using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace GDPR_Download.API.Views.Install
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
                
        protected void SubmitForm(EventArgs e, object sender)
        {

        }

        public class InstallInfo
        {
            public string SQLServer { get; set; }
            public string SQLPort { get; set; }
            public string SQLDatabaseName { get; set; }
            public string SQLUsername { get; set; }
            public string SQLPassword { get; set; }
            public string SMTPServer { get; set; }
            public string SMTPPort { get; set; }
            public string SMTPUsername { get; set; }
            public string SMTPPassword { get; set; }
            public string licensePath { get; set; }
            public string Domain { get; set; }
            public string uiPath { get; set; }
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

        private void AddUpdateConnectionString(string newConString, InstallInfo settingsInfo)
        {
            bool isNew = false;
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Web.Config");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList list = doc.DocumentElement.SelectNodes("connectionStrings/add[@name='DefaultConnection']");

            string newSmtpConnection = "Host=" + settingsInfo.SMTPServer;
            if (settingsInfo.SMTPPort != "")
            {
                newSmtpConnection += ";Port=" + settingsInfo.SMTPPort;
            }
            if (settingsInfo.SMTPUsername != "")
            {
                newSmtpConnection += ";Username=" + settingsInfo.SMTPUsername;
            }
            if (settingsInfo.SMTPPassword != "")
            {
                newSmtpConnection += ";Password=" + settingsInfo.SMTPPassword;
            }

            XmlNode appSettingsNode = doc.SelectSingleNode("configuration/appSettings");

            foreach (XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes != null)
                {
                    if (childNode.Attributes["key"].Value == "smtp")
                    {
                        childNode.Attributes["value"].Value = newSmtpConnection;
                    }
                    else if (childNode.Attributes["key"].Value == "domain")
                    {
                        childNode.Attributes["value"].Value = settingsInfo.Domain;
                    }
                    else if (childNode.Attributes["key"].Value == "licensekeypath")
                    {
                        childNode.Attributes["value"].Value = settingsInfo.licensePath;
                    }
                }
            }

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
            if (settingsInfo.SQLPort != "")
            {
                conStringBuilder.DataSource = settingsInfo.SQLServer + ":" + settingsInfo.SQLPort;
            }
            else
            {
                conStringBuilder.DataSource = settingsInfo.SQLServer;
            }
            conStringBuilder.InitialCatalog = settingsInfo.SQLDatabaseName;
            conStringBuilder.IntegratedSecurity = true;
            if (settingsInfo.SQLUsername != "")
            {
                conStringBuilder.UserID = settingsInfo.SQLUsername;
            }
            if (settingsInfo.SQLPassword != "")
            {
                conStringBuilder.Password = settingsInfo.SQLPassword;
            }
            node.Attributes["connectionString"].Value = conStringBuilder.ConnectionString;
            if (isNew)
            {
                doc.DocumentElement.SelectNodes("connectionStrings")[0].AppendChild(node);
            }
            doc.Save(path);
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            InstallInfo installInfo = new InstallInfo
            {
                SQLServer = sql_Server.Text,
                SQLPort = sql_Port.Text,
                SQLDatabaseName = sql_Database.Text,
                SQLPassword = sql_Password.Text,
                SQLUsername = sql_Username.Text,
                SMTPServer = smtp_Server.Text,
                SMTPPort = smtp_Port.Text,
                SMTPUsername = smtp_Username.Text,
                SMTPPassword = smtp_Password.Text,
                Domain = Domain.Text,
                licensePath = licensePath.Text,
                uiPath = uiPath.Text
            };


            string sqlString = "";
            if (installInfo.SQLPort != "")
            {
                sqlString = "Server=" + installInfo.SQLServer + ":" + installInfo.SQLPort + ";Database=" + installInfo.SQLDatabaseName + ";Integrated Security=True;";
            }
            else
            {
                sqlString = "Server=" + installInfo.SQLServer + ";Database=" + installInfo.SQLDatabaseName + ";Integrated Security=True;";
            }

            if (installInfo.SQLUsername != "")
            {
                sqlString += "User id=" + installInfo.SQLUsername;
            }
            if (installInfo.SQLPassword != "")
            {
                sqlString += ";Password=" + installInfo.SQLPassword;
            }
            AddUpdateConnectionString(sqlString, installInfo);
            UpdateUIFiles(installInfo.uiPath, Request.Url.Authority);
        }

        public void UpdateUIFiles(string uiPath, string domain)
        {
            FileInfo file = new FileInfo(uiPath + @"\dataConstants.js");
            if(file.Exists == true)
            {
                var originalLines = File.ReadAllLines(file.FullName);

                var updatedLines = new List<string>();
                foreach (var line in originalLines)
                {
                    string newLine = line.Replace("*", domain);
                    updatedLines.Add(newLine);
                }

                File.WriteAllLines(file.FullName, updatedLines);
            }
        }
    }
}