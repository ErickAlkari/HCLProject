using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HCL_MiniProject.Startup))]
namespace HCL_MiniProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
