using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Command;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Dotpay.Front.Validators;
using Dotpay.Front.ViewModel;
using Dotpay.FrontQueryService;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Dotpay.Front.YiSheng;
using Dotpay.ViewModel;

namespace Dotpay.Front.Controllers
{
    public class RecordsController : BaseController
    {
        [HttpGet]
        [Route("~/records/deposit")]
        public ActionResult Deposit()
        {
            return View();
        }

        [HttpPost]
        [Route("~/records/deposit/list")]
        public async Task<ActionResult> Deposit(DateTime? startDate, DateTime? endDate, int draw, int start, int length)
        {
            if (startDate.HasValue && !endDate.HasValue)
            {
                var result = PagerListModel<DepositTransactionListViewModel>.Create(new List<DepositTransactionListViewModel>(), draw, 0, 0);
                return Json(result);
            }
            else
            {
                var page = start / length;
                var pagesize = length;
                var query = IoC.Resolve<ITransactionQuery>();
                IEnumerable<DepositTransactionListViewModel> dataList = null;
                var count =
                    await query.CountDepositTransaction(this.CurrentUser.AccountId, startDate.Value, endDate.Value.AddDays(1));
                if (count > 0)
                {
                    dataList =
                        await
                            query.GetDepositTransaction(this.CurrentUser.AccountId, startDate.Value, endDate.Value.AddDays(1), page,
                                pagesize);

                    dataList.ForEach(row =>
                    {
                        row.PaywayName = this.Lang(row.Payway.ToLangString());
                        row.StatusName = this.Lang(row.Status.ToLangString());
                    });
                }

                var result = PagerListModel<DepositTransactionListViewModel>.Create(dataList, draw, count, count);
                return Json(result);
            }
        }
        [Route("~/records/transfer")]
        public ActionResult Transfer()
        {
            return View();
        }

        [HttpPost]
        [Route("~/records/transfer/list")]
        public async Task<ActionResult> Transfer(DateTime? startDate, DateTime? endDate, int draw, int start, int length)
        {
            if (startDate.HasValue && !endDate.HasValue)
            {
                var result = PagerListModel<TransferTransactionListViewModel>.Create(new List<TransferTransactionListViewModel>(), draw, 0, 0);
                return Json(result);
            }
            else
            {
                var page = start / length;
                var pagesize = length;
                var query = IoC.Resolve<ITransactionQuery>();
                IEnumerable<TransferTransactionListViewModel> dataList = null;
                var count =
                    await query.CountTransferTransaction(this.CurrentUser.AccountId, startDate.Value, endDate.Value.AddDays(1));
                if (count > 0)
                {
                    dataList =
                        await
                            query.GetTransferTransaction(this.CurrentUser.AccountId, startDate.Value, endDate.Value.AddDays(1), page,
                                pagesize);

                    dataList.ForEach(row =>
                    {
                        row.Payway= this.Lang(row.Payway);
                        row.Status = this.Lang(row.Status);
                        row.Bank = this.Lang(row.Bank); 
                    });
                }

                var result = PagerListModel<TransferTransactionListViewModel>.Create(dataList, draw, count, count);
                return Json(result);
            }
        }
    }
}