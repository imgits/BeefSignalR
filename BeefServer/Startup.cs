using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.StaticFiles;
using System.Security.Claims;
using System.IO;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin;
using System.Net;

namespace BeefServer
{
    class Startup
    {
        string AccountAuthPath = "/Account/Auth";
        string AccountLoginPath = "/Account/Login";
        string AccountLogoutPath = "/Account/Logout";

        string AppRootPath = null;
        string ScriptsRootPath = null;
        string HtmlRootPath = null;
        string ResourceRootPath = null;

        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            //GlobalHost.HubPipeline.RequireAuthentication();

            ConfigFileSystem(app);

            ConfigureAuth(app);

            app.MapSignalR();
        }


        void ConfigFileSystem(IAppBuilder app)
        {
            string AppRootPath = Path.Combine(Environment.CurrentDirectory, @"..\..");
            var fileOptions = new StaticFileOptions
            {
                FileSystem = new PhysicalFileSystem(AppRootPath),
            };
            app.UseStaticFiles(fileOptions);

            ScriptsRootPath = Path.Combine(AppRootPath, "scripts");
            HtmlRootPath = Path.Combine(AppRootPath, "View");
            ResourceRootPath = Path.Combine(AppRootPath, "res");
        }

        string MapRequestPath(string virtual_path)
        {
            string real_path = virtual_path;
            if (virtual_path.StartsWith("/Scripts/",StringComparison.OrdinalIgnoreCase))
            {
                real_path = ScriptsRootPath + virtual_path.Substring("/Scripts".Length);
            }
            else if (virtual_path.StartsWith("/res/", StringComparison.OrdinalIgnoreCase))
            {
                real_path = ScriptsRootPath + virtual_path.Substring("/res".Length);
            }
            return real_path;
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            CookieAuthenticationOptions options = new CookieAuthenticationOptions()
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString(AccountLoginPath),
                LogoutPath = new PathString(AccountLogoutPath),
            };

            app.UseCookieAuthentication(options);

            app.Use(async (context, next) =>
            {
                await AccountAuth(context, next);
            });
        }

        async Task AccountAuth(IOwinContext context,Func<Task>next)
        {
            string real_path = MapRequestPath(context.Request.Path.Value);
            Console.WriteLine(context.Request.Method + " " + context.Request.Path.Value + "==>" + real_path);
            var redirectUri = context.Request.Query["ReturnUrl"];
            if (context.Request.Path.Value== AccountLoginPath)
            { 
                if (context.Request.Method == "POST")
                {// POST Account/Login
                    var form = await context.Request.ReadFormAsync();
                    var username = form["Username"];
                    var password = form["Password"];
                    if (ValidateUserCredentials(username,password))
                    {
                        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);
                        identity.AddClaim(new Claim(ClaimTypes.Name, username));
                        context.Authentication.SignIn(identity);

                        redirectUri = redirectUri ?? "/index.html";
                        context.Response.Redirect(redirectUri);

                        //context.Response.StatusCode = 200;
                        //context.Response.ReasonPhrase = "OK";
                        //context.Response.Write("OK");
                        //context.Response.Redirect("/");

                    }
                    else
                    {
                        var redirect = AccountLoginPath;
                        if (!String.IsNullOrEmpty(redirectUri))
                        {
                            redirect += "?ReturnUrl=" + WebUtility.UrlEncode(redirectUri);
                        }
                        context.Response.Redirect(redirect);
                        //context.Response.StatusCode = 401;
                        //context.Response.Write("OK");
                    }
                }
                else
                {//GET /Account/Login
                    context.Response.ContentType = "text/html";
                    string loginForm = File.ReadAllText("../../View/Account/Login.html");
                    await context.Response.WriteAsync(loginForm);
                }
            }
            else if (context.Request.Path.Value == AccountLogoutPath)
            {
                context.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                redirectUri = redirectUri ?? AccountLoginPath;
                context.Response.Redirect(redirectUri);
            }
            else if (context.Request.User == null || !context.Request.User.Identity.IsAuthenticated)
            {
                if (context.Request.Path.Value.EndsWith(".js") ||
                    context.Request.Path.Value.EndsWith(".css") ||
                    context.Request.Path.Value.EndsWith(".ico"))
                {
                    await next();
                    return;
                }
                context.Response.Redirect(AccountLoginPath);
            }
            else if (context.Request.Path.Value == "/")
            {
                context.Response.Redirect("../View/index.html");
            }
            else
            {
                await next();
            }
        }

        private bool ValidateUserCredentials(string username, string password)
        {
            return username == "user" && password == "password";
        }
    }
}
