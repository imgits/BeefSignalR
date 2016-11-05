using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BeefMVC.Startup))]
namespace BeefMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
