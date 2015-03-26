using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dotpay.Web.Startup))]
namespace Dotpay.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
