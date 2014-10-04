using DotPay.Web.Filters;
using System.Web;
using System.Web.Mvc;

namespace DotPay.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new DotPayHandlerErrorAttribute());
            filters.Add(new Authentication());
        }
    }
}