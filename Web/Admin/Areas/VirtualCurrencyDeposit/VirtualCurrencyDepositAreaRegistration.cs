using System.Web.Mvc;

namespace DotPay.Web.Admin.Areas.VirtualCurrencyDeposit
{
    public class VirtualCurrencyDepositAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "VirtualCurrencyDeposit";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "VirtualCurrencyDeposit_default",
                "VirtualCurrencyDeposit/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[]{"DotPay.Web.Admin.Areas.VirtualCurrencyDeposit.Controllers"}
            );
        }
    }
}