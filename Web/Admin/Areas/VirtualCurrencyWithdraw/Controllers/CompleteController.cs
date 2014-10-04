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
using FullCoin.Web.Admin.Filters;
namespace FullCoin.Web.Admin.Areas.VirtualCurrencyWithdraw.Controllers
{
    public class CompleteController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult btc()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult ifc()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult nxt()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult ltc()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult doge()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult str()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetWithdrawCountBySearch(int? userID, CurrencyType currencyType, WithdrawState state)
        {

            var count = IoC.Resolve<IWithdrawQuery>().CountCompleteWithdrawBySearch(userID, currencyType, state);

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetWithdrawBySearch(int? userID, CurrencyType currencyType, WithdrawState state, int page)
        {
            var currencies = IoC.Resolve<IWithdrawQuery>().GetCompleteWithdrawBySearch(userID, currencyType, state, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }
      
    }
}