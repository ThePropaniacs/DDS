using DDSDemoDAL;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DDSDemo.Startup))]
namespace DDSDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
