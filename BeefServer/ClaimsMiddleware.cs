using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BeefServer
{
    class ClaimsMiddleware : OwinMiddleware
    {
        public ClaimsMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            string username = context.Request.Headers.Get("username");
            string password = context.Request.Headers.Get("password");

            if (!String.IsNullOrEmpty(password))
            {
                if (username == "user" && password == "password")
                {
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);
                    identity.AddClaim(new Claim(ClaimTypes.Name, username));
                    context.Authentication.SignIn(identity);
                    //context.Response.StatusCode = 200;
                    //context.Response.ReasonPhrase = "OK";
                    //context.Response.Write("OK");
                    //context.Response.Redirect("/");

                    context.Request.User = new ClaimsPrincipal(identity);

                    Console.WriteLine("Auer Auth OK");
                    //var claims = new List<Claim>
                    //{
                    //    new Claim(ClaimTypes.Authentication, "true")
                    //};

                    //var claimsIdentity = new ClaimsIdentity(claims);
                    //context.Request.User = new ClaimsPrincipal(claimsIdentity);

                }
                else
                {
                    context.Response.StatusCode = 401;
                    context.Response.Write("ERROR");
                }
            }

            return Next.Invoke(context);
        }
    }
}
