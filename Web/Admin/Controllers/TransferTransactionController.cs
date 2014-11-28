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
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetPendingTransferTransaction(TransactionState state, PayWay payWay, int page)
        {
            var count = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionCountBySearch(TransactionState.Pending, payWay);
            var result = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Pending, payWay, page, Constants.DEFAULT_PAGE_COUNT);
            return Json(new { count = count, result = result });
        }

    }
}
