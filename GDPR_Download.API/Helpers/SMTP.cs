using GDPR_Download.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using static GDPR_Download.API.Controllers.AdminController;

namespace GDPR_Download.Helpers
{
    public class SMTP
    {
        public bool SendMail(UploadInfoDOT uploadInfo, string senderEmail, string url)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    SMTPSettings smtpInfo = new SMTPSettings();
                    string[] smtpArray = ConfigurationManager.AppSettings["smtp"].Split(';');

                    foreach (string smtpInformation in smtpArray)
                    {
                        string key = smtpInformation.Split('=')[0];
                        string value = smtpInformation.Split('=')[1];

                        if (key == "Host")
                        {
                            smtpInfo.Host = value;
                        }
                        else if (key == "Port")
                        {
                            smtpInfo.Port = value;
                        }
                        else if (key == "Username")
                        {
                            smtpInfo.Username = value;
                        }
                        else if (key == "Password")
                        {
                            smtpInfo.Password = value;
                        }
                    }

                    int port = 25;
                    int.TryParse(smtpInfo.Port, out port);

                    client.Connect(smtpInfo.Host, port);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(smtpInfo.Username, smtpInfo.Password);
                    MimeMessage message = new MimeMessage();
                    message.To.Add(new MailboxAddress("Daniel Frykman", uploadInfo.ReciverEmail));
                    message.From.Add(new MailboxAddress("Daniel Frykman", senderEmail));
                    message.Subject = uploadInfo.Subject;
                    var bodyBuilder = new BodyBuilder();
                    string bodyHtml = API.Properties.Resources.template;
                    bodyHtml = bodyHtml.Replace("{subject}", uploadInfo.Subject).Replace("{body}", uploadInfo.Description).Replace("{filename}", uploadInfo.FileName.Split('.')[0]).Replace("{downloadUrl}", url + "api/upload/download/" + uploadInfo.FileName.Split('.')[0]);
                    bodyBuilder.HtmlBody = bodyHtml;
                
                    message.Body = bodyBuilder.ToMessageBody();
                    client.Send(message);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SendLoginMail(string reciverEmail, string senderEmail, string url, string loginToken)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    SMTPSettings smtpInfo = new SMTPSettings();
                    string[] smtpArray = ConfigurationManager.AppSettings["smtp"].Split(';');

                    foreach (string smtpInformation in smtpArray)
                    {
                        string key = smtpInformation.Split('=')[0];
                        string value = smtpInformation.Split('=')[1];

                        if (key == "Host")
                        {
                            smtpInfo.Host = value;
                        }
                        else if (key == "Port")
                        {
                            smtpInfo.Port = value;
                        }
                        else if (key == "Username")
                        {
                            smtpInfo.Username = value;
                        }
                        else if (key == "Password")
                        {
                            smtpInfo.Password = value;
                        }
                    }

                    int port = 25;
                    int.TryParse(smtpInfo.Port, out port);

                    client.Connect(smtpInfo.Host, port);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(smtpInfo.Username, smtpInfo.Password);
                    MimeMessage message = new MimeMessage();
                    message.To.Add(new MailboxAddress("Daniel Frykman", reciverEmail));
                    message.From.Add(new MailboxAddress("Daniel Frykman", senderEmail));
                    message.Subject = "(DAJA) Upload request";
                    var bodyBuilder = new BodyBuilder();
                    string bodyHtml = API.Properties.Resources.loginTemplate;
                    bodyHtml = bodyHtml.Replace("{subject}", "(DAJA) Upload request").Replace("{body}", "Test").Replace("{filename}", "test").Replace("{loginUrl}", url + loginToken);
                    bodyBuilder.HtmlBody = bodyHtml;
                    
                    message.Body = bodyBuilder.ToMessageBody();
                    client.Send(message);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public bool SendMailOld(UploadInfoDOT uploadInfo, string senderEmail, string url)
        //{
        //    try
        //    {
        //        SMTPSettings smtpInfo = new SMTPSettings();
        //        string[] smtpArray = ConfigurationManager.AppSettings["smtp"].Split(';');

        //        foreach(string smtpInformation in smtpArray)
        //        {
        //            string key = smtpInformation.Split('=')[0];
        //            string value = smtpInformation.Split('=')[1];

        //            if (key == "Host")
        //            {
        //                smtpInfo.Host = value;
        //            }
        //            else if (key == "Port")
        //            {
        //                smtpInfo.Port = value;
        //            }
        //            else if (key == "Username")
        //            {
        //                smtpInfo.Username = value;
        //            }
        //            else if (key == "Password")
        //            {
        //                smtpInfo.Password = value;
        //            }

        //        }

        //        int port = 25;
        //        int.TryParse(smtpInfo.Port, out port);

        //        SmtpClient client = new SmtpClient
        //        {
        //            Port = port,
        //            //DeliveryMethod = SmtpDeliveryMethod.Network,
        //            //UseDefaultCredentials = false,
        //            Credentials = new System.Net.NetworkCredential(smtpInfo.Username, smtpInfo.Password),
        //            Host = smtpInfo.Host

        //        };

        //        MailMessage mail = new MailMessage(senderEmail, uploadInfo.ReciverEmail)
        //        {
        //            Subject = uploadInfo.Subject,
        //            IsBodyHtml = true,
        //            Body = "<h1>" + uploadInfo.Subject + "</h1>" + uploadInfo.Description + "<br><br><a href=\"" + url + "api/upload/download/" + uploadInfo.FileName.Split('.')[0] + "\">Ladda ner fil</a></br></br></br>Skickat via Daja-Solutions"
        //        };

        //        client.Send(mail);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}