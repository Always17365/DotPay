using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Admin.Controllers;
using Dotpay.Admin.Fliter;
using Dotpay.Admin.ViewModel;
using Dotpay.AdminCommand;
using Dotpay.AdminQueryService;
using Dotpay.Common;
using Dotpay.Common.Enum;

namespace Dotpay.Admin.Controllers
{
    public class TransferController : BaseController
    {
        #region Page
        [Route("~/ajax/dotpaytofi/pending")]
        [AllowRoles(Role = ManagerType.TransferManager)]
        public ActionResult DotpayToFiPendingList()
        {
            return PartialView();
        }

        [Route("~/ajax/dotpaytofi/complete")]
        [AllowRoles(Role = ManagerType.TransferManager)]
        public ActionResult DotpayToFiCompleteList()
        {
            return PartialView();
        }
        [Route("~/ajax/dotpaytofi/fail")]
        [AllowRoles(Role = ManagerType.TransferManager)]
        public ActionResult DotpayToFiFailList()
        {
            return PartialView();
        }

        #endregion

        #region 数据获取

        [HttpPost]
        [AllowRoles(Role = ManagerType.TransferManager)]
        [Route("~/ajax/dotpaytofi/tx/pending/list")]
        public async Task<ActionResult> GetPendingList(string email, int draw, int start, int length)
        {
            email = email.NullSafe().Trim();
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<ITransferTransactionQuery>();
            IEnumerable<TransferFromDotpayToFiListViewModel> dataList = null;
            var count = await query.CountPendingDotpayToFiTransferTx(email);
            if (count > 0)
            {
                dataList = await query.GetPendingDotpayToFiTransferTx(email, page, pagesize);
                dataList.ForEach(d =>
                {
                    d.BankName = Language.LangHelpers.Lang(d.Bank.ToLangString());
                });
            }

            var result = PagerListModel<TransferFromDotpayToFiListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(result);
        }

        [HttpPost]
        [AllowRoles(Role = ManagerType.TransferManager)]
        [Route("~/ajax/dotpaytofi/tx/complete/list")]
        public async Task<ActionResult> GetCompleteList(string email, string sequenceNo, string transferNo, int draw, int start, int length)
        {
            email = email.NullSafe().Trim();
            sequenceNo = sequenceNo.NullSafe().Trim();
            transferNo = transferNo.NullSafe().Trim();
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<ITransferTransactionQuery>();
            IEnumerable<TransferFromDotpayToFiListViewModel> dataList = null;
            var count = await query.CountDotpayToFiTransferTx(email, sequenceNo, transferNo, TransferTransactionStatus.Completed);
            if (count > 0)
            {
                dataList = await query.GetDotpayToFiTransferTx(email, sequenceNo, transferNo, TransferTransactionStatus.Completed, page, pagesize);
                dataList.ForEach(d =>
                {
                    d.BankName = Language.LangHelpers.Lang(d.Bank.ToLangString());
                });
            }

            var result = PagerListModel<TransferFromDotpayToFiListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(result);
        }

        [HttpPost]
        [AllowRoles(Role = ManagerType.TransferManager)]
        [Route("~/ajax/dotpaytofi/tx/fail/list")]
        public async Task<ActionResult> GetFailList(string email, string sequenceNo, int draw, int start, int length)
        {
            email = email.NullSafe().Trim();
            sequenceNo = sequenceNo.NullSafe().Trim(); 
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<ITransferTransactionQuery>();
            IEnumerable<TransferFromDotpayToFiListViewModel> dataList = null;
            var count = await query.CountDotpayToFiTransferTx(email, sequenceNo, string.Empty, TransferTransactionStatus.Failed);
            if (count > 0)
            {
                dataList = await query.GetDotpayToFiTransferTx(email, sequenceNo, string.Empty, TransferTransactionStatus.Failed, page, pagesize);
                dataList.ForEach(d =>
                {
                    d.BankName = Language.LangHelpers.Lang(d.Bank.ToLangString());
                });
            }

            var result = PagerListModel<TransferFromDotpayToFiListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(result);
        }
        #endregion

        #region 锁定转账
        [HttpPost]
        [AllowRoles(Role = ManagerType.TransferManager)]
        [Route("~/ajax/dotpaytofi/tx/lock")]
        public async Task<ActionResult> LockTransferTransaction(Guid txid)
        {
            var result = DotpayJsonResult.UnknowFail;

            try
            {
                var cmd = new LockTransferTransactionCommand(txid, this.CurrentUser.ManagerId);
                await this.CommandBus.SendAsync(cmd);

                if (cmd.CommandResult == ErrorCode.None)
                    result = DotpayJsonResult.Success;
                else if (cmd.CommandResult == ErrorCode.HasNoPermission)
                    result = DotpayJsonResult.CreateFailResult("无权进行此操作");
                else if (cmd.CommandResult == ErrorCode.TranasferTransactionIsLockedByOther)
                    result = DotpayJsonResult.CreateFailResult("已被其它管理员锁定");
                else if (cmd.CommandResult == ErrorCode.AutomaticTranasferTransactionCanNotProccessByManager)
                    result = DotpayJsonResult.CreateFailResult("自动处理的转账，无需人工干预，看到这个消息请通知管理员");
            }
            catch (Exception ex)
            {
                Log.Error("LockTransferTransaction", ex);
            }
            return Json(result);
        }
        #endregion

        #region 确认转账成功
        [HttpPost]
        [AllowRoles(Role = ManagerType.TransferManager)]
        [Route("~/ajax/dotpaytofi/tx/confirm")]
        public async Task<ActionResult> ConfirmTransferTransaction(Guid txid, string transferNo, decimal amount)
        {
            var result = DotpayJsonResult.UnknowFail;

            try
            {
                var cmd = new ConfirmTransferTransactionSuccessCommand(txid, amount, transferNo, this.CurrentUser.ManagerId);
                await this.CommandBus.SendAsync(cmd);

                if (cmd.CommandResult == ErrorCode.None)
                    result = DotpayJsonResult.Success;
                else if (cmd.CommandResult == ErrorCode.HasNoPermission)
                    result = DotpayJsonResult.CreateFailResult("无权进行此操作");
                else if (cmd.CommandResult == ErrorCode.TranasferTransactionAmountNotMatch)
                    result = DotpayJsonResult.CreateFailResult("金额不匹配");
                else if (cmd.CommandResult == ErrorCode.TranasferTransactionIsLockedByOther)
                    result = DotpayJsonResult.CreateFailResult("已被其它管理员锁定");
                else if (cmd.CommandResult == ErrorCode.AutomaticTranasferTransactionCanNotProccessByManager)
                    result = DotpayJsonResult.CreateFailResult("自动处理的转账，无需人工干预，看到这个消息请通知管理员");
            }
            catch (Exception ex)
            {
                Log.Error("LockTransferTransaction", ex);
            }
            return Json(result);
        }
        #endregion

        #region 确认转账失败
        [HttpPost]
        [AllowRoles(Role = ManagerType.TransferManager)]
        [Route("~/ajax/dotpaytofi/tx/confirmfail")]
        public async Task<ActionResult> ConfirmTransferTransactionFail(Guid txid, string reason)
        {
            var result = DotpayJsonResult.UnknowFail;

            try
            {
                var cmd = new ConfirmTransferTransactionFailCommand(txid, reason, this.CurrentUser.ManagerId);
                await this.CommandBus.SendAsync(cmd);

                if (cmd.CommandResult == ErrorCode.None)
                    result = DotpayJsonResult.Success;
                else if (cmd.CommandResult == ErrorCode.HasNoPermission)
                    result = DotpayJsonResult.CreateFailResult("无权进行此操作");
                else if (cmd.CommandResult == ErrorCode.TranasferTransactionIsLockedByOther)
                    result = DotpayJsonResult.CreateFailResult("已被其它管理员锁定");
                else if (cmd.CommandResult == ErrorCode.AutomaticTranasferTransactionCanNotProccessByManager)
                    result = DotpayJsonResult.CreateFailResult("自动处理的转账，无需人工干预，看到这个消息请通知管理员");
            }
            catch (Exception ex)
            {
                Log.Error("LockTransferTransaction", ex);
            }
            return Json(result);
        }
        #endregion
    }
}