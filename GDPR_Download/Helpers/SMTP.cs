using GDPR_Download.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace GDPR_Download.Helpers
{
    public class SMTP
    {
        public bool SendMail(UploadInfoDOT uploadInfo)
        {
            try
            {
                bool emailFound = false;
                string[] senderEmailsAllowed = File.ReadAllLines(ConfigurationManager.AppSettings["EmailListPath"]);

                int users = int.Parse(ConfigurationManager.AppSettings["Users"]);

                if(senderEmailsAllowed.Count() > users)
                {
                    return false;

                }

                foreach (string Email in senderEmailsAllowed)
                {
                    if(uploadInfo.SenderEmail == Email)
                    {
                        emailFound = true;
                    }
                }

                if (emailFound == true)
                {
                    MailMessage mail = new MailMessage(uploadInfo.SenderEmail, uploadInfo.ReciverEmail);
                    SmtpClient client = new SmtpClient();

                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //client.UseDefaultCredentials = true;
                    client.Credentials = new System.Net.NetworkCredential("gdpr_upload@infintyit.se", "Jio98JL9j!");
                    client.Host = "mail.infintyit.se";

                    mail.Subject = uploadInfo.Subject;
                    mail.IsBodyHtml = true;
                    mail.Body = "<h1>" + uploadInfo.FileName + "</h1>" + uploadInfo.Description;

                    client.Send(mail);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}