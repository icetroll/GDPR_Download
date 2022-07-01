using System;
using System.Threading.Tasks;
using GDPR_Download.Helpers;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GDPR_Download.Startup))]

namespace GDPR_Download
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LicenseSystem license = new LicenseSystem();
            int valid = license.ValidateLicense();

            if(valid == 3)
            {
                app.Run(context =>
                {
                    return context.Response.WriteAsync("You license key have expired");
                });
            }
            else if (valid == 2)
            {
                app.Run(context =>
                {
                    return context.Response.WriteAsync("Could not validate your license key");
                });
            }
        }
    }
}
