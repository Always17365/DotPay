using System.Web.Mvc;
using FC.Framework;
using DotPay.Common;
using DotPay.QueryService;
using DotPay.Web.Admin.Controllers;
using System;
using DotPay.Web.Admin.Filters;
using DotPay.Command;

namespace DotPay.Web.Admin.Areas.VirtualCurrencyDeposit.Controllers
{
    public class DepositController : BaseController
    {

        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult DepositVirtualDeposit()
        {
            return View();
        }

        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult ReceivePayMentTransaction()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult CountVirtualCurrencyDepositBySearch(int? userID, DateTime? starttime, DateTime? endtime, CurrencyType currency, DepositState state)
        {
            var count = IoC.Resolve<IDepositQuery>().CountVirtualCurrencyDepositBySearch(userID, starttime, endtime, currency, state);

            return Json(count);
        }
        [HttpPost]
        public ActionResult GetVirtualCurrencyDepositBySearch(int? userID, DateTime? starttime, DateTime? endtime, CurrencyType currency, DepositState state, int page)
        {
            var users = IoC.Resolve<IDepositQuery>().GetVirtualCurrencyDepositBySearch(userID, starttime, endtime, currency, state, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(users);
        }

        
        [HttpPost]
        public ActionResult GetVirtualCurrencyDepositByConfirmation(int? userID, DateTime? starttime, DateTime? endtime, CurrencyType currency, DepositState state, int page)
        {
            var users = IoC.Resolve<IDepositQuery>().GetVirtualCurrencyDepositBySearch(userID, starttime, endtime, currency, state, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(users);
        }

        /***************************************************/
        [HttpPost]
        public ActionResult CountReceivePayMentTransactionBySearch(CurrencyType currencyType)
        {
            var users = IoC.Resolve<IDepositQuery>().CountReceivePayMentTransactionBySearch(currencyType, DepositState.Verify);

            return Json(users);
        }

    }
}