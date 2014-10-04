using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FullCoin.QueryService;
using FC.Framework;
using System.Web.Helpers;
using FullCoin.ViewModel;
using FullCoin.Command;
using FullCoin.Common;

namespace FullCoin.Web.Admin.Controllers
{

    public class WithdrawController : BaseController
    {
        public ActionResult WaitSubmit()
        {
            return View();
        }
        public ActionResult Examine()
        {
            return View();
        }
        public ActionResult Processing()
        {
            return View();
        }
        public ActionResult Complete()
        {
            return View();
        }

        public ActionResult UndoExamine()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetWithdrawCountBySearch(int? amount, string transfer, string action)
        {

            var count = IoC.Resolve<IWithdrawQuery>().CountWithdrawBySearch(amount, transfer.NullSafe(), ParseToWithdrawState(action));

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetWithdrawBySearch(int? amount, string transfer, string action, int page)
        {
            var currencies = IoC.Resolve<IWithdrawQuery>().GetWithdrawBySearch(amount, transfer.NullSafe(), ParseToWithdrawState(action), page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }
        
        [HttpPost]
        public ActionResult Verify(int withdrawID, string memo="")
        {
            try
            {
                var cmd = new CNYWithdrawVerify(withdrawID, memo, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult WaitSubmit(int withdrawID, string memo = "")
        {
            try
            {
                var cmd = new SubmitCNYWithdrawToProcess(withdrawID, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult Success(int withdrawID, int transferAccountID, string transferNo )
        {
            try
            {
                var cmd = new CNYWithdrawMarkAsSuccess(withdrawID, transferAccountID, transferNo, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult Transferfail(int withdrawID, string memo = "")
        {
            try
            {
                var cmd = new CNYWithdrawMarkAsTransferFail(withdrawID, memo, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult UndoApplication(int withdrawID, string memo = "")
        {
            try
            {
                var cmd = new CNYWithdrawCancel(withdrawID, memo, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }

        [HttpPost]
        public ActionResult ChangerUserBank(int withdrawID, Bank bank, string bankAccount, string bankAddress, string openBankName)
        {
            try
            {
#if !DEBUG
                var cmd = new CNYWithdrawModifyReceiverBankAccount(withdrawID, bank, bankAccount, bankAddress, openBankName, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);
#endif
                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
 

        #region 私有方法
        private WithdrawState ParseToWithdrawState(string stateString)
        {
            var state = default(WithdrawState);

            switch (stateString)
            {
                case "wv":
                    state = WithdrawState.WaitVerify;
                    break;

                case "e":
                    state = WithdrawState.WaitSubmit;
                    break;

                case "p":
                    state = WithdrawState.Processing;
                    break;
                case "c":
                    state = WithdrawState.Complete;
                    break;
                case "f":
                    state = WithdrawState.Fail;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return state;
        }
        #endregion

    }
}
