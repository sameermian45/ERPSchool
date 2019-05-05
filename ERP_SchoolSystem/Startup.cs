using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ERP_SchoolSystem.Startup))]
namespace ERP_SchoolSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
