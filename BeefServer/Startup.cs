using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Cookies;
using System.Security.Claims;

namespace BeefServer
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();

            app.Use(typeof(ClaimsMiddleware));

            CookieAuthenticationOptions options = new CookieAuthenticationOptions()
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = CookieAuthenticationDefaults.LoginPath,
                LogoutPath = CookieAuthenticationDefaults.LogoutPath,
                //LoginPath = new PathString("/Auth/Login"),
                //LogoutPath = new PathString("/Auth/Logout"),
            };

            app.UseCookieAuthentication(options);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Contains(options.LoginPath.Value))
                {
                    if (context.Request.Method == "POST")
                    {
                        var form = await context.Request.ReadFormAsync();
                        var username = form["Username"];
                        var password = form["Password"];
                        if (username == "ahai" && password == "pass")
                        {
                            var identity = new ClaimsIdentity(options.AuthenticationType);
                            identity.AddClaim(new Claim(ClaimTypes.Name, username));
                            context.Authentication.SignIn(identity);
                            context.Response.StatusCode = 200;
                            context.Response.Write("OK");
                        }
                        else
                        {
                            context.Response.StatusCode = 401;
                            context.Response.Write("OK");
                        }
                    }
                }
                else
                {
                    await next();
                }
            });

        }
    }
}
