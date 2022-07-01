using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GDPR_Download.Helpers
{
    public class LicenseSystem
    {
        internal string GenerateLicense(string licenseKeyInformation)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(licenseKeyInformation));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public int ValidateLicense()
        {
            string licenseKey = File.ReadAllText(ConfigurationManager.AppSettings["LicenseKeyPath"]).Replace("\r", "").Replace("\n", "");
            string domain = ConfigurationManager.AppSettings["Domain"];
            string users = ConfigurationManager.AppSettings["Users"];

            DateTime target = DateTime.Now.AddYears(-2);
            int match = 0;
            while (match == 0)
            {
                target = target.AddDays(1);
                string newKey = "$" + domain + "$" + target.ToString("yyyy-MM-dd") + "$" + users + "$";
                string licenseMatch = GenerateLicense(newKey);

                if(licenseMatch == licenseKey)
                {
                    if (target < DateTime.Now)
                    {
                        match = 3;
                    }
                    else
                    {
                        match = 1;
                    }
                }

                if (target.Year == DateTime.Now.AddYears(4).Year)
                {
                    match = 2;
                }
            }

            return match;
        }
    }
}