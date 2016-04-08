using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SystemFarma.Startup))]
namespace SystemFarma
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
