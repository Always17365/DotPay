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
using Dotpay.AdminQueryService;
using Dotpay.Common;
using Dotpay.Common.Enum;

namespace Dotpay.Admin.Controllers
{
    public class TransferController : BaseController
    {
        [Route("~/ajax/dotpaytofi/pending")]
        [AllowRoles(Role = ManagerType.TransferManager)]
        public ActionResult DotpayToFi()
        {
            return PartialView();
        }

        [HttpGet]
        [AllowRoles(Role = ManagerType.TransferManager)]
        [Route("~/ajax/dotpaytofi/tx/pending/list")]
        public async Task<ActionResult> GetPendingList(string loginName, int draw, int start, int length)
        {
            loginName = loginName.NullSafe().Trim();
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<ITransferTransactionQuery>();
            IEnumerable<TransferFromDotpayToFiListViewModel> dataList = null;
            var count = await query.CountPendingDotpayToFiTransferTx(loginName);
            if (count > 0)
            {
                dataList = await query.GetPendingDotpayToFiTransferTx(loginName, page, pagesize);
            }

            var result = PagerListModel<TransferFromDotpayToFiListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(result);
        }

        [HttpGet]
        [AllowRoles(Role = ManagerType.TransferManager)]
        [Route("~/ajax/dotpaytofi/tx/complete/list")]
        public async Task<ActionResult> GetCompleteList(string loginName, string sequenceNo, string transferNo, int draw, int start, int length)
        {
            loginName = loginName.NullSafe().Trim();
            sequenceNo = sequenceNo.NullSafe().Trim();
            transferNo = transferNo.NullSafe().Trim();
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<ITransferTransactionQuery>();
            IEnumerable<TransferFromDotpayToFiListViewModel> dataList = null;
            var count = await query.CountDotpayToFiTransferTx(loginName, sequenceNo, transferNo, TransferTransactionStatus.Completed);
            if (count > 0)
            {
                dataList = await query.GetDotpayToFiTransferTx(loginName, sequenceNo, transferNo, TransferTransactionStatus.Completed, page, pagesize);
            }

            var result = PagerListModel<TransferFromDotpayToFiListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(null);
        }

        [HttpGet]
        [AllowRoles(Role = ManagerType.TransferManager)]
        [Route("~/ajax/dotpaytofi/tx/fail/list")]
        public async Task<ActionResult> GetFailList(string loginName, string sequenceNo, string transferNo, int draw, int start, int length)
        {
            loginName = loginName.NullSafe().Trim();
            sequenceNo = sequenceNo.NullSafe().Trim();
            transferNo = transferNo.NullSafe().Trim();
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<ITransferTransactionQuery>();
            IEnumerable<TransferFromDotpayToFiListViewModel> dataList = null;
            var count = await query.CountDotpayToFiTransferTx(loginName, sequenceNo, transferNo, TransferTransactionStatus.Failed);
            if (count > 0)
            {
                dataList = await query.GetDotpayToFiTransferTx(loginName, sequenceNo, transferNo, TransferTransactionStatus.Failed, page, pagesize);
            }

            var result = PagerListModel<TransferFromDotpayToFiListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(null);
        }
    }
}