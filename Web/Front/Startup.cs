using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DotPay.Web.Startup))]
namespace DotPay.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
