using System.Web;
using System.Web.Mvc;
using Dotpay.Admin.Fliter;

namespace Dotpay.Admin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticationAttribute());
        }
    }
}
