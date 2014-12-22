using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.QueryService;
using FC.Framework;
using System.Web.Helpers;
using DotPay.ViewModel;
using DotPay.Command;
using DotPay.Common;
using DotPay.Web.Admin.Filters;

namespace DotPay.Web.Admin.Controllers
{

    public class TransferTransactionController : BaseController
    {
        public ActionResult Pending()
        {
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Fail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SelectRippleTxid(string txid)
        {
            var result = IoC.Resolve<ITransferTransactionQuery>().SelectRippleTxid(txid, PayWay.Alipay);
            if (result != null)
            {
                return Json(result);
            }
            result = IoC.Resolve<ITransferTransactionQuery>().SelectRippleTxid(txid, PayWay.Tenpay);
            if (result != null)
            {
                return Json(result);
            }
            return(null);
        }
        [HttpPost]
        public ActionResult GetPendingTransferTransaction( )
        { 
            var result1 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Pending, PayWay.Alipay, 1, 10);
            var result2 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Init, PayWay.Alipay, 1, 10);
            var result3 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Pending, PayWay.Tenpay, 1, 10);
            var result4 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Init, PayWay.Tenpay, 1, 10);
            var result = result1.Union<TransferTransaction>(result2).Union<TransferTransaction>(result3).Union<TransferTransaction>(result4);
            return Json(result.Take(10).OrderByDescending(q => q.CreateAt));
        }

        [HttpPost]
        public ActionResult GetPendingTransferTransaction(PayWay payWay, int page)
        {
            var count1 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionCountBySearch("", 0, "", null, null, TransactionState.Pending, payWay);
            var count2 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionCountBySearch("", 0, "", null, null, TransactionState.Init, payWay);
            var result1 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Pending, payWay, page, Constants.DEFAULT_PAGE_COUNT);
            var result2 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Init, payWay, page, Constants.DEFAULT_PAGE_COUNT);
            return Json(new { count = count1 + count2, result = result1.Union<TransferTransaction>(result2) });
        }
        [HttpPost]
        public ActionResult GetSuccessTransferTransaction(string account, int? amount, string txid, DateTime? starttime, DateTime? endtime, PayWay payWay, int page)
        {
            var count = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionCountBySearch(account, amount, txid, starttime, endtime, TransactionState.Success, payWay);
            var result = IoC.Resolve<ITransferTransactionQuery>().SelectTransferTransactionBySearch(account, amount, txid, starttime, endtime, TransactionState.Success, payWay, page, Constants.DEFAULT_PAGE_COUNT);
            return Json(new { count = count, result = result });
        }
        [HttpPost]
        public ActionResult GetFailTransferTransaction(string account, int? amount, string txid, DateTime? starttime, DateTime? endtime, PayWay payWay, int page)
        {
            var count = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionCountBySearch(account, amount, txid, starttime, endtime, TransactionState.Fail, payWay);
            var result = IoC.Resolve<ITransferTransactionQuery>().SelectTransferTransactionBySearch(account, amount, txid, starttime, endtime, TransactionState.Fail, payWay, page, Constants.DEFAULT_PAGE_COUNT);
            return Json(new { count = count, result = result });
        }

        [HttpPost]
        public ActionResult ThirdPartyPaymentTransferComplete(int transferId, decimal amount, string transferNo, PayWay payway)
        {
            try
            {
                var cmd = new ThirdPartyPaymentTransferComplete(transferId, transferNo, amount, payway, this.CurrentUser.UserID);
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

        [HttpPost]
        public ActionResult ThirdPartyPaymentTransferFail(int transferId, string reason, PayWay payway)
        {
            try
            {
                var cmd = new ThirdPartyPaymentTransferFail(transferId, reason, payway, this.CurrentUser.UserID);
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

        [HttpPost]
        public ActionResult MarkThirdPartyPaymentTransferProcessing(int tppTransferId, PayWay payway)
        {
            try
            {
                var cmd = new MarkThirdPartyPaymentTransferProcessing(tppTransferId, payway, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);
                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                if (ex.ErrorCode == (int)ErrorCode.TransferTransactionNotInit)
                    return Json(new JsonResult(-3));
                else return Json(new JsonResult(ex.ErrorCode));
            }

        }
    }
}
