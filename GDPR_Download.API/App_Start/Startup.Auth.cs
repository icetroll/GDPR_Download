using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using GDPR_Download.API.Providers;
using GDPR_Download.API.Models;
using GDPR_Download.Helpers;
using System.Data.Entity.Migrations;
using System.Configuration;
using System.Web.Mvc;

namespace GDPR_Download.API
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static OAuthAuthorizationServerOptions OAuthOptions2 { get; private set; }

        public static string PublicClientId { get; private set; }

        void CheckLicense(IAppBuilder app)
        {
            LicenseSystem license = new LicenseSystem();
            int valid = license.ValidateLicense();

            if (valid == 3)
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

        void UpdateFiles()
        {

        }

        void InstallPage(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LicenseKeyPath"]))
                {
                    if (!context.Request.Uri.AbsoluteUri.ToLower().Contains("install"))
                    {
                        context.Response.Redirect("install/index.aspx");

                        return;
                    }
                }
                else
                {
                    if (context.Request.Uri.AbsoluteUri.ToLower().Contains("install"))
                    {
                        //context.Response.Redirect(context.Request.Uri.Authority);

                        return;
                    }
                }
                await next();
            });
        }


        public static bool IsDatabaseInstalled()
        {
            var key = ConfigurationManager.AppSettings["SQLConnectionSettings:SqlServerIp"];
            return !string.IsNullOrEmpty(key);
        }

        bool firstRun = false;

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            if (firstRun == false)
            {
                InstallPage(app);
                CheckLicense(app);
                UpdateFiles();
                firstRun = true;
            }

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //ConfigureAuth(app);

            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };


            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}
