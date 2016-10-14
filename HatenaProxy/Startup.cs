using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HatenaProxy.Startup))]
namespace HatenaProxy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
