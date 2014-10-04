using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using DotPay.QueryService;
using FC.Framework;
using DotPay.ViewModel;
using DotPay.Command;
using DotPay.Common;
using DotPay.Web.Admin.Filters;

namespace DotPay.Web.Admin.Controllers
{
    public class VirtualCoinStatisController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager,ManagerType.SystemManager,ManagerType.Monitor)]
        public ActionResult Index()
        {
            ViewData["NowDateTime"] = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            return View();
        }
        /*******************************************************************************************************************************************/
        
        [HttpPost]
        public ActionResult GetResults(CurrencyType currencyType)
        {
            object[] result = new object[4];
            result[0] = IoC.Resolve<IVirtualCoinStatisticsQuery>().GetWalletByVirtualCoin(currencyType);
            result[1] = IoC.Resolve<IVirtualCoinStatisticsQuery>().GetRateOfFlowByVirtualCoin(currencyType);
            result[2] = IoC.Resolve<IVirtualCoinStatisticsQuery>().TransactionStatistics(currencyType);
            result[3] = IoC.Resolve<IVirtualCoinStatisticsQuery>().OrderStatistics(currencyType);
            return Json(result);
        }
    }
}
