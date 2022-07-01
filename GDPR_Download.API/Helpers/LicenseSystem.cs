using GDPR_Download.API.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GDPR_Download.Helpers
{
    public class LicenseSystem
    {
        public static int usersAllowed = 0;

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

        void AddFirstUser()
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                if (dbContext.Users.Count() < 1)
                {
                    var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));

                    string roleName = "Administratör";
                    IdentityResult roleResult;

                    if (!RoleManager.RoleExists(roleName))
                    {
                        roleResult = RoleManager.Create(new IdentityRole(roleName));
                    }

                    roleName = "Användare";
                    if (!RoleManager.RoleExists(roleName))
                    {
                        roleResult = RoleManager.Create(new IdentityRole(roleName));
                    }

                    roleName = "Gäst";
                    if (!RoleManager.RoleExists(roleName))
                        roleResult = RoleManager.Create(new IdentityRole(roleName));

                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));

                    ApplicationUser firstUser = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin",
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    PasswordHasher passwordHasher = new PasswordHasher();
                    firstUser.PasswordHash = passwordHasher.HashPassword("password");
                    dbContext.Users.AddOrUpdate(firstUser);
                    dbContext.SaveChanges();
                    var role = UserManager.AddToRole(firstUser.Id, "Administratör");
                }
            }
        }

        public int ValidateLicense()
        {
            int match = 0;
            if (!File.Exists(ConfigurationManager.AppSettings["LicenseKeyPath"]))
            {
                match = 4;
                return match;
            }
            string licenseKey = File.ReadAllText(ConfigurationManager.AppSettings["LicenseKeyPath"]).Replace("\r", "").Replace("\n", "");
            string domain = ConfigurationManager.AppSettings["Domain"];
            string users = ConfigurationManager.AppSettings["Users"];

            DateTime target = DateTime.Now.AddYears(-2);
            while (match == 0)
            {
                target = target.AddDays(1);
                for (int i = 0; i <= 1000; i++)
                {
                    string newKey = "$" + domain.Replace("http://", "").Replace("https://", "") + "$" + target.ToString("yyyy-MM-dd") + "$" + i + "$";
                    string licenseMatch = GenerateLicense(newKey);

                    if (licenseMatch == licenseKey)
                    {
                        if (target < DateTime.Now)
                        {
                            match = 3;
                            return match;
                        }
                        else
                        {
                            match = 1;
                            usersAllowed = i;
                            AddFirstUser();
                            return match;
                        }
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