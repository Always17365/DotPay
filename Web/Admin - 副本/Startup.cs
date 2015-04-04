using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dotpay.Admin.Startup))]
namespace Dotpay.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
          
        }
    }
}
