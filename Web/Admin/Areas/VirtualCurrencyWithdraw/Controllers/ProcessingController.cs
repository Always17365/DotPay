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

    public class ProcessingController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult Btc()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult Ifc()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult Nxt()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult Ltc()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult Doge()
        {
            return View();
        }

        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult str()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetWithdrawCountBySearch(int? userID, CurrencyType currencyType, VirtualCoinTxState state)
        {

            var count = IoC.Resolve<IWithdrawQuery>().CountVirtualCoinWithdrawBySearch(userID,currencyType, state);

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetWithdrawBySearch(int? userID, CurrencyType currencyType, VirtualCoinTxState state, int page)
        {
            var currencies = IoC.Resolve<IWithdrawQuery>().GetVirtualCoinWithdrawBySearch(userID,currencyType, state, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }
       

    }
}