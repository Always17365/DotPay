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
    public class CancelController : Controller
    {
        // GET: VirtualCurrencyWithdraw/Cancel
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetWithdrawCountBySearch(int? userID, CurrencyType currencyType)
        {

            var count = IoC.Resolve<IWithdrawQuery>().CountDestroyWithdrawBySearch(userID, currencyType, WithdrawState.Cancel);

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetWithdrawBySearch(int? userID, CurrencyType currencyType, int page)
        {
            var currencies = IoC.Resolve<IWithdrawQuery>().GetDestroyWithdrawBySearch(userID, currencyType, WithdrawState.Cancel, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }



    }
}