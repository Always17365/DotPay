using System.Web;
using System.Web.Mvc;
using Dotpay.Front.Fliter;

namespace Dotpay.Front
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
