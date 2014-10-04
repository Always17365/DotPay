using System.Web.Mvc;

namespace DotPay.Web.Admin.Areas.Deposit
{
    public class DepositAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CNYDeposit";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CNYDeposit_default",
                "CNYDeposit/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "DotPay.Web.Admin.Areas.CNYDeposit.Controllers" }
            );
        }
    }
}