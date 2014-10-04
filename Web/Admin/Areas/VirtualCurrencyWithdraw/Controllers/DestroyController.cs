using FC.Framework;
using FullCoin.Command;
using FullCoin.Common;
using FullCoin.QueryService;
using FullCoin.Web.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FullCoin.Web.Admin.Filters;
namespace FullCoin.Web.Admin.Areas.VirtualCurrencyWithdraw.Controllers
{
    public class DestroyController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetWithdrawCountBySearch(int? userID, CurrencyType currencyType)
        {

            var count = IoC.Resolve<IWithdrawQuery>().CountDestroyWithdrawBySearch(userID, currencyType, WithdrawState.Fail);

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetWithdrawBySearch(int? userID, CurrencyType currencyType, int page)
        {
            var currencies = IoC.Resolve<IWithdrawQuery>().GetDestroyWithdrawBySearch(userID, currencyType, WithdrawState.Fail, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }
        /*************************************************************************************************************************/
        [HttpPost]
        public ActionResult CancelVirtualCoinWithdraw(string withdrawUniqueID, string memo, CurrencyType currency)
        {
            try
            {
                var cmd = new CancelVirtualCoinWithdraw(withdrawUniqueID, this.CurrentUser.UserID ,memo, currency);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }


    }
}