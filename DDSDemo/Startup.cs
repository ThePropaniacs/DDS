using DDSDemoDAL;
using System.Web.Services.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Microsoft.Extensions.DependencyInjection;

[assembly: OwinStartupAttribute(typeof(DDSDemo.Startup))]
namespace DDSDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            
        }
    }
}
