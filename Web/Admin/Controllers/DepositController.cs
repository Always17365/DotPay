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
    public class DepositController : BaseController
    {
        [Route("~/ajax/deposit/tx/alipaypendinglist")]
        [AllowRoles(Role = ManagerType.DepositManager)]
        public ActionResult AlipayPendingList()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowRoles(Role = ManagerType.DepositManager)]
        [Route("~/ajax/deposit/tx/alipay/list/pending")]
        public async Task<ActionResult> GetAlipayPendingDepositList(string email, string sequenceNo, int draw, int start, int length)
        {
            email = email.NullSafe().Trim();
            sequenceNo = sequenceNo.NullSafe().Trim();
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<IDepositTransactionQuery>();
            IEnumerable<DepositListViewModel> dataList = null;
            var count = await query.CountDepositTransactionBySearch(email, sequenceNo, Payway.Alipay, DepositStatus.Started);
            if (count > 0)
            {
                dataList = await query.GetDepositTransactionBySearch(email, sequenceNo, Payway.Alipay, DepositStatus.Started, page, pagesize);
            }

            var result = PagerListModel<DepositListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(result);
        }

        #region 待处理的充值

        [Route("~/ajax/deposit/tx/pendinglist")]
        [AllowRoles(Role = ManagerType.DepositManager)]
        public ActionResult PendingList()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowRoles(Role = ManagerType.DepositManager)]
        [Route("~/ajax/deposit/tx/list/pending")]
        public async Task<ActionResult> GetPendingDepositList(string email, string sequenceNo, int draw, int start, int length)
        {
            email = email.NullSafe().Trim();
            sequenceNo = sequenceNo.NullSafe().Trim();
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<IDepositTransactionQuery>();
            IEnumerable<DepositListViewModel> dataList = null;
            var count = await query.CountDepositTransactionBySearch(email, sequenceNo, null, DepositStatus.Started);
            if (count > 0)
            {
                dataList = await query.GetDepositTransactionBySearch(email, sequenceNo, null, DepositStatus.Started, page, pagesize);
            }

            var result = PagerListModel<DepositListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(result);
        }

        #region 确认充值有效
        [HttpPost]
        [AllowRoles(Role = ManagerType.DepositManager)]
        [Route("~/ajax/deposit/confirm")]
        public async Task<ActionResult> ConfirmDeposit(Guid depositId, decimal amount, string transferNo)
        {
            var result = DotpayJsonResult.UnknowFail;
            if (!depositId.IsNullOrEmpty() && amount > 0 && !string.IsNullOrEmpty(transferNo))
            {
                try
                {
                    var cmd = new ConfirmDepositCommand(depositId, amount, transferNo, this.CurrentUser.ManagerId);
                    await this.CommandBus.SendAsync(cmd);

                    if (cmd.CommandResult == ErrorCode.None)
                        result = DotpayJsonResult.Success;
                    else if (cmd.CommandResult == ErrorCode.HasNoPermission)
                        result = DotpayJsonResult.CreateFailResult("无权进行此操作");
                    else if (cmd.CommandResult == ErrorCode.DepositAmountNotMatch)
                        result = DotpayJsonResult.CreateFailResult("充值金额不匹配");
                }
                catch (Exception ex)
                {
                    Log.Error("ConfirmDeposit Exception", ex);
                }
            }
            return Json(result);
        }
        #endregion
        #endregion

        #region 已完成的充值
        [Route("~/ajax/deposit/tx/completelist")]
        [AllowRoles(Role = ManagerType.DepositManager)]
        public ActionResult CompleteList()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowRoles(Role = ManagerType.DepositManager)]
        [Route("~/ajax/deposit/tx/list/complete")]
        public async Task<ActionResult> GetCompleteDepositList(string email, string sequenceNo, int draw, int start, int length)
        {
            email = email.NullSafe().Trim();
            sequenceNo = sequenceNo.NullSafe().Trim();
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<IDepositTransactionQuery>();
            IEnumerable<DepositListViewModel> dataList = null;
            var count = await query.CountDepositTransactionBySearch(email, sequenceNo, null, DepositStatus.Completed);
            if (count > 0)
            {
                dataList = await query.GetDepositTransactionBySearch(email, sequenceNo, null, DepositStatus.Completed, page, pagesize);
            }

            var result = PagerListModel<DepositListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(result);
        }
        #endregion

        #region 已失败的充值列表(此处暂时没有数据，考虑以后作为已清理的充值数据)
        [Route("~/ajax/deposit/tx/faillist")]
        [AllowRoles(Role = ManagerType.DepositManager)]
        public ActionResult FailList()
        {
            return PartialView();
        }
        [HttpPost]
        [AllowRoles(Role = ManagerType.DepositManager)]
        [Route("~/ajax/deposit/tx/list/fail")]
        public async Task<ActionResult> GetFailDepositList(string email,string sequenceNo, int draw, int start, int length)
        {
            email = email.NullSafe().Trim();
            sequenceNo = sequenceNo.NullSafe().Trim();
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<IDepositTransactionQuery>();
            IEnumerable<DepositListViewModel> dataList = null;
            var count = await query.CountDepositTransactionBySearch(email, sequenceNo, null, DepositStatus.Fail);
            if (count > 0)
            {
                dataList = await query.GetDepositTransactionBySearch(email, sequenceNo, null, DepositStatus.Fail, page, pagesize);
            }

            var result = PagerListModel<DepositListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(result);
        }
        #endregion
    }
}