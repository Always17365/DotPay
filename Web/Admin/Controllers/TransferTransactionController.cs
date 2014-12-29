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
        public ActionResult GetTransferTransactionByRippleTxid(string txid)
        {
            var result = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionByRippleTxid(txid, PayWay.Alipay);
            if (result != null)
            {
                return Json(result);
            }
            result = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionByRippleTxid(txid, PayWay.Tenpay);
            if (result != null)
            {
                return Json(result);
            }
            return (null);
        }
        [HttpPost]
        public ActionResult GetLastTenTransferTransaction()
        {
            var result = IoC.Resolve<ITransferTransactionQuery>().GetLastTwentyTransferTransaction();
            return Json(result);
        }
        [HttpPost]
        public ActionResult GetPendingTransferTransaction(PayWay payWay, int page)
        {
            var count1 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionCountBySearch("", 0, "", null, null, TransactionState.Pending, payWay);
            var count2 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionCountBySearch("", 0, "", null, null, TransactionState.Init, payWay);
            var result1 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch("", 0, "", null, null, TransactionState.Pending, payWay, "ASC", page, Constants.DEFAULT_PAGE_COUNT);
            var result2 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch("", 0, "", null, null, TransactionState.Init, payWay, "ASC", page, Constants.DEFAULT_PAGE_COUNT);
            var result = from TransferTransaction in result1.Union<TransferTransaction>(result2)
                         select new TransferTransaction
                         {
                             Account = TransferTransaction.Account,
                             Amount = TransferTransaction.Amount,
                             CreateAt = TransferTransaction.CreateAt,
                             DoneAt = TransferTransaction.DoneAt,
                             ID = TransferTransaction.ID,
                             Memo = TransferTransaction.Memo,
                             PayWay = TransferTransaction.PayWay,
                             RealName = TransferTransaction.RealName,
                             Reason = TransferTransaction.Reason,
                             SequenceNo = FormatString(TransferTransaction.SequenceNo, 20, ' '),
                             SourcePayway = TransferTransaction.SourcePayway,
                             State = TransferTransaction.State,
                             TransferNo = FormatString(TransferTransaction.TransferNo, 20, ' '),
                             TxId = FormatString(TransferTransaction.TxId, 32, ' ')
                         };

            return Json(new { count = count1 + count2, result = result.OrderByDescending(q => q.CreateAt) });
        }

        [HttpPost]
        public ActionResult GetSuccessTransferTransaction(string account, int? amount, string txid, DateTime? starttime, DateTime? endtime, PayWay payWay, int page)
        {
            var count = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionCountBySearch(account, amount, txid, starttime, endtime, TransactionState.Success, payWay);
            var result = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(account, amount, txid, starttime, endtime, TransactionState.Success, payWay, "DESC", page, Constants.DEFAULT_PAGE_COUNT);
            result = from TransferTransaction in result
                     select new TransferTransaction
                     {
                         Account = TransferTransaction.Account,
                         Amount = TransferTransaction.Amount,
                         CreateAt = TransferTransaction.CreateAt,
                         DoneAt = TransferTransaction.DoneAt,
                         ID = TransferTransaction.ID,
                         Memo = TransferTransaction.Memo,
                         PayWay = TransferTransaction.PayWay,
                         RealName = TransferTransaction.RealName,
                         Reason = TransferTransaction.Reason,
                         SequenceNo = FormatString(TransferTransaction.SequenceNo, 20, ' '),
                         SourcePayway = TransferTransaction.SourcePayway,
                         State = TransferTransaction.State,
                         TransferNo = FormatString(TransferTransaction.TransferNo, 20, ' '),
                         TxId = FormatString(TransferTransaction.TxId, 32, ' ')
                     };
            return Json(new { count = count, result = result.OrderByDescending(q => q.CreateAt) });
        }
        [HttpPost]
        public ActionResult GetFailTransferTransaction(string account, int? amount, string txid, DateTime? starttime, DateTime? endtime, PayWay payWay, int page)
        {
            var count = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionCountBySearch(account, amount, txid, starttime, endtime, TransactionState.Fail, payWay);
            var result = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(account, amount, txid, starttime, endtime, TransactionState.Fail, payWay, "DESC", page, Constants.DEFAULT_PAGE_COUNT);
            result = from TransferTransaction in result
                     select new TransferTransaction
                     {
                         Account = TransferTransaction.Account,
                         Amount = TransferTransaction.Amount,
                         CreateAt = TransferTransaction.CreateAt,
                         DoneAt = TransferTransaction.DoneAt,
                         ID = TransferTransaction.ID,
                         Memo = TransferTransaction.Memo,
                         PayWay = TransferTransaction.PayWay,
                         RealName = TransferTransaction.RealName,
                         Reason = TransferTransaction.Reason,
                         SequenceNo = FormatString(TransferTransaction.SequenceNo, 20, ' '),
                         SourcePayway = TransferTransaction.SourcePayway,
                         State = TransferTransaction.State,
                         TransferNo = FormatString(TransferTransaction.TransferNo, 20, ' '),
                         TxId = FormatString(TransferTransaction.TxId, 32, ' ')
                     };
            return Json(new { count = count, result = result.OrderByDescending(q => q.CreateAt) });
        }

        [HttpPost]
        public ActionResult ThirdPartyPaymentTransferComplete(int transferId, decimal amount, string transferNo, PayWay payway, PayWay transferPay)
        {
            try
            {
                var cmd = new ThirdPartyPaymentTransferComplete(transferId, transferNo, amount, payway, transferPay, this.CurrentUser.UserID);
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
        public string FormatString(string str, int len, char chr)
        {
            string result = "";
            if (str.Length <= len) return str;
            for (int i = 0; i < (str.Length / len); i++)
                result += str.Substring(i * len, len) + chr;
            if (str.Length % len != 0) result += str.Substring((str.Length / len) * len);
            result = result.Trim(chr);
            return result;
        }
    }
}
