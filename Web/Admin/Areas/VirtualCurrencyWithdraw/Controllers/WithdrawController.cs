using FC.Framework;
using DotPay.Command;
using DotPay.Common;
using DotPay.QueryService;
using DotPay.Web.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.Web.Admin.Filters;
namespace DotPay.Web.Admin.Areas.VirtualCurrencyWithdraw.Controllers
{
    public class WithdrawController : BaseController
    {

        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult Index()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult verifyindex()
        {
            return View();
        }   
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult completeindex()
        {
            return View();
        }        

        
        [HttpPost]
        public ActionResult GetWithdrawCountBySearch(int? userID,CurrencyType currencyType, VirtualCoinTxState state)
        {

            var count = IoC.Resolve<IWithdrawQuery>().CountVirtualCoinWithdrawBySearch(userID,currencyType, state);

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetWithdrawBySearch(int? userID,CurrencyType currencyType, VirtualCoinTxState state, int page)
        {
            var currencies = IoC.Resolve<IWithdrawQuery>().GetVirtualCoinWithdrawBySearch(userID,currencyType, state, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }
        /*************************************************************************************************************************/

        [HttpPost]
        public ActionResult CompleteVirtualCoinWithdraw(string withdrawUniqueID, string txid, CurrencyType currency)
        {
            try
            {
                var cmd = new CompleteVirtualCoinWithdraw(withdrawUniqueID, txid, 0, currency);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }               
        
        [HttpPost]
        public ActionResult Verify(int withdrawID, CurrencyType currencyType, string memo = "")
        {
            try
            {
                var cmd = new VirtualCoinWithdrawVerify(withdrawID, memo, currencyType, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult CancelVirtualCoinWithdraw(string withdrawUniqueID, string memo, CurrencyType currency)
        {
            try
            {
                var cmd = new CancelVirtualCoinWithdraw(withdrawUniqueID, this.CurrentUser.UserID, memo, currency);
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