using System.Web.Mvc;

namespace FullCoin.Web.Admin.Areas.Withdraw
{
    public class WithdrawAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Withdraw";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Withdraw_default",
                "Withdraw/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "FullCoin.Web.Admin.Areas.Withdraw.Controllers" }
            );
        }
    }
}