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

namespace FullCoin.Web.Admin.Areas.Withdraw.Controllers
{
    public class CompleteController : BaseController
    {
        public ActionResult btc()
        {
            return View();
        }
        public ActionResult ifc()
        {
            return View();
        }
        public ActionResult nxt()
        {
            return View();
        }
        public ActionResult ltc()
        {
            return View();
        }
        public ActionResult doge()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetWithdrawCountBySearch(int? userID, string action, string state)
        {

            var count = IoC.Resolve<ICheckPendingQuery>().CountWithdrawBySearch(userID, ParseToAction(action), ParseToWithdrawState(state));

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetWithdrawBySearch(int? userID, string action, string state, int page)
        {
            var currencies = IoC.Resolve<ICheckPendingQuery>().GetWithdrawBySearch(userID, ParseToAction(action), ParseToWithdrawState(state), page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }
        /*************************************************************************************************************************/
        [HttpPost]
        public ActionResult Verify(int withdrawID, string action, string memo = "")
        {
            try
            {
                var cmd = new VirtualCoinWithdrawVerify(withdrawID, memo, ParseToCurrency(action), this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        /***************************************************************************************************************************/
        #region 私有方法
        private string ParseToAction(string action)
        {

            switch (action)
            {
                case "btc":
                    action = "btcwithdraw";
                    break;
                case "ifc":
                    action = "ifcwithdraw";
                    break;
                case "ltc":
                    action = "ltcwithdraw";
                    break;
                case "nxt":
                    action = "nxtwithdraw";
                    break;
                case "doge":
                    action = "dogewithdraw";
                    break;

                default:
                    throw new NotImplementedException();
            }

            return action;
        }
        private WithdrawState ParseToWithdrawState(string stateString)
        {
            var state = default(WithdrawState);

            switch (stateString)
            {
                case "1":
                    state = WithdrawState.WaitVerify;
                    break;
                case "2":
                    state = WithdrawState.WaitSubmit;
                    break;
                case "3":
                    state = WithdrawState.Processing;
                    break;
                case "4":
                    state = WithdrawState.Complete;
                    break;
                case "5":
                    state = WithdrawState.Fail;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return state;
        }
        private CurrencyType ParseToCurrency(string stateString)
        {
            var state = default(CurrencyType);

            switch (stateString)
            {
                case "btc":
                    state = CurrencyType.BTC;
                    break;
                case "cny":
                    state = CurrencyType.CNY;
                    break;
                case "doge":
                    state = CurrencyType.DOGE;
                    break;
                case "ifc":
                    state = CurrencyType.IFC;
                    break;
                case "ltc":
                    state = CurrencyType.LTC;
                    break;
                case "nxt":
                    state = CurrencyType.NXT;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return state;
        }
        #endregion

    }
}