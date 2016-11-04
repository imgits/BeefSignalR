using Microsoft.Owin;
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

            if (!String.IsNullOrEmpty(username))
            {
                var authenticated = ((username == "ahai") && (password=="pass")) ? "true":"false";

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Authentication, authenticated)
                    };

                var claimsIdentity = new ClaimsIdentity(claims);
                context.Request.User = new ClaimsPrincipal(claimsIdentity);
            }

            return Next.Invoke(context);
        }
    }
}
