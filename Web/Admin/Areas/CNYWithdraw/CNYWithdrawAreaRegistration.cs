using System.Web.Mvc;

namespace DotPay.Web.Admin.Areas.Withdraw
{
    public class CNYWithdrawAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CNYWithdraw";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CNYWithdraw_default",
                "CNYWithdraw/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "DotPay.Web.Admin.Areas.CNYWithdraw.Controllers" }
            );
        }
    }
}