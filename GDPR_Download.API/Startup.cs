using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using GDPR_Download.API.Data;
using GDPR_Download.Common;
using Microsoft.Owin;
using Owin;
using System.Windows.Forms;
using System.Threading;

[assembly: OwinStartup(typeof(GDPR_Download.API.Startup))]

namespace GDPR_Download.API
{
    public partial class Startup
    {
        static string StoragePath = ConfigurationManager.AppSettings["StoragePath"];

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Thread t = new Thread(() => StartChecking());
            t.Start();
        }
        private void StartChecking()
        {
            while (true)
            {
                CheckFiles();
                Thread.Sleep(300000);
            }
        }

        public void CheckFiles()
        {
            try
            {
                using (SiteDbContext dbContext = new SiteDbContext())
                {
                    List<FileModel> files = dbContext.Files.Where(x => x.Opened == true).ToList();
                    foreach (FileModel file in files)
                    {
                        if (Math.Floor(file.UploadDate.Subtract(DateTime.Now).TotalHours) >= 2)
                        {
                            if (File.Exists(StoragePath + file.FileName))
                                File.Delete(StoragePath + file.FileName);
                            dbContext.Files.Remove(file);
                            dbContext.SaveChanges();
                        }
                    }

                    List<FileModel> removeFiles = dbContext.Files.Where(x => x.Opened == false).ToList();
                    foreach (FileModel file in removeFiles)
                    {
                        if (file.UploadDate.Subtract(DateTime.Now).Days >= 7)
                        {
                            if (File.Exists(StoragePath + file.FileName))
                                File.Delete(StoragePath + file.FileName);

                            // Add error message for not finding file

                            dbContext.Files.Remove(file);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
