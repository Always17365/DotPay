using System.Web.Mvc;
using FC.Framework;
using DotPay.Command;
using DotPay.Common;
using DotPay.QueryService;
using DotPay.Web.Admin.Controllers;
using DotPay.Web.Admin.Filters;

namespace DotPay.Web.Admin.Areas.CNYDeposit.Controllers
{
    public class DepositController : BaseController
    {

        [VerifyIsManager(ManagerType.SuperManager, ManagerType.DepositOfficer, ManagerType.Monitor)]
        public ActionResult DepositCNY()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetDepositCountBySearch(int? amountsearch, string email)
        {
            var count = IoC.Resolve<IDepositQuery>().CountDepositBySearch(amountsearch, email.NullSafe());

            return Json(count);
        }
        [HttpPost]
        public ActionResult GetDepositBySearch(int? amountsearch, string email, int page)
        {
            var users = IoC.Resolve<IDepositQuery>().GetDepositBySearch(amountsearch, email.NullSafe(), page, Constants.DEFAULT_PAGE_COUNT);

            return Json(users);
        }
        [HttpPost]
        public ActionResult UndoCompleteCNYDeposit(int depositID, int UserID)
        {
            int num = IoC.Resolve<IDepositQuery>().QueryCompleteCNYDeposit(depositID, UserID);

            if (num <= 0)
            {
                return Json(new JsonResult(-3));
            }
            try
            {
                var cmd = new UndoCompleteCNYDeposit(depositID, UserID);
                this.CommandBus.Send(cmd);
                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                if (ex.ErrorCode == (int)ErrorCode.NoPermission)
                    return Json(new JsonResult(-3));
                else return Json(new JsonResult(ex.ErrorCode));
            }
        }

    }
}