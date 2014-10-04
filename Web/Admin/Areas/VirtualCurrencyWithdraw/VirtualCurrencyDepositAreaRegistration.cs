using System.Web.Mvc;

namespace DotPay.Web.Admin.Areas.VirtualCurrencyWithdraw
{
    public class VirtualCurrencyWithdrawAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "VirtualCurrencyWithdraw";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "VirtualCurrencyWithdraw_default",
                "VirtualCurrencyWithdraw/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "DotPay.Web.Admin.Areas.VirtualCurrencyWithdraw.Controllers" }
            );

        }
    }
}