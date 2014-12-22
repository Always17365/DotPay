using DotPay.Web.Admin.Filters;
using System.Web;
using System.Web.Mvc;

namespace DotPay.Web.Admin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (!DotPay.Common.Config.Debug)
                filters.Add(new Authentication());

            filters.Add(new HandleErrorAttribute());
        }
    }
}