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
    public class LogsController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult UserLockLog()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult UserLoginLog()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult UserAssignRoleLog()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult CurrencyLog()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult VipSettingLog()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult btcdepositprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult btcwithdrawprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult capitalaccountopreatelog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult cnydepositprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult cnywithdrawprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult depositauthorizationuselog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult depositauthorizelog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult dogedepositprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult dogewithdrawprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult fundsourcelog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult ifcdepositprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult ifcwithdrawprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult ltcdepositprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult ltcwithdrawprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult marketsettinglog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult nxtdepositprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult nxtwithdrawprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult fbcdepositprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult fbcwithdrawprocesslog() { return View(); }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        /*------------------------------------------------------------------------------------------*/
        [HttpPost]
        public ActionResult GetUserLoginCountBySearch(string email)
        {
            var count = IoC.Resolve<ILogsQuery>().GetUserLoginCountBySearch(email);

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetUserLoginBySearch(string email, int page)
        {
            var result = IoC.Resolve<ILogsQuery>().GetUserLoginBySearch(email, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(result);
        }

        /*------------------------------------------------------------------------------------------*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">LOG数据库表名</param>
        /// <param name="email">email</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLogsCountBySearch(string action, string email)
        {
            var count = IoC.Resolve<ILogsQuery>().GetLogsCountBySearch(ParseToAction(action), email);

            return Json(count);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">LOG数据库表名</param>
        /// <param name="email"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLogsBySearch(string action, string email, int page)
        {
            var result = IoC.Resolve<ILogsQuery>().GetLogsBySearch(ParseToAction(action), email, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(result);
        }

        #region 私有方法
        private string ParseToAction(string tableString)
        {
            string state = string.Empty;
            switch (tableString)
            {
                case "btcdepositprocesslog":
                    state = "btcdepositprocesslog";
                    break;
                case "btcwithdrawprocesslog":
                    state = "btcwithdrawprocesslog";
                    break;
                case "capitalaccountopreatelog":
                    state = "capitalaccountopreatelog";
                    break;
                case "cnydepositprocesslog":
                    state = "cnydepositprocesslog";
                    break;
                case "cnywithdrawprocesslog":
                    state = "cnywithdrawprocesslog";
                    break;
                case "currencylog":
                    state = "currencylog";
                    break;
                case "depositauthorizationuselog":
                    state = "depositauthorizationuselog";
                    break;
                case "depositauthorizelog":
                    state = "depositauthorizelog";
                    break;
                case "dogedepositprocesslog":
                    state = "dogedepositprocesslog";
                    break;
                case "dogewithdrawprocesslog":
                    state = "dogewithdrawprocesslog";
                    break;
                case "fundsourcelog":
                    state = "fundsourcelog";
                    break;
                case "ifcdepositprocesslog":
                    state = "ifcdepositprocesslog";
                    break;
                case "ifcwithdrawprocesslog":
                    state = "ifcwithdrawprocesslog";
                    break;
                case "loginlog":
                    state = "loginlog";
                    break;
                case "ltcdepositprocesslog":
                    state = "ltcdepositprocesslog";
                    break;
                case "ltcwithdrawprocesslog":
                    state = "ltcwithdrawprocesslog";
                    break;
                case "marketsettinglog":
                    state = "marketsettinglog";
                    break;
                case "nxtdepositprocesslog":
                    state = "nxtdepositprocesslog";
                    break;
                case "nxtwithdrawprocesslog":
                    state = "nxtwithdrawprocesslog";
                    break;
                case "userassignrolelog":
                    state = "userassignrolelog";
                    break;
                case "userlocklog":
                    state = "userlocklog";
                    break;
                case "vipsettinglog":
                    state = "vipsettinglog";
                    break;
                case "fbcdepositprocesslog":
                    state = "fbcdepositprocesslog";
                    break;
                case "fbcwithdrawprocesslog":
                    state = "fbcwithdrawprocesslog";
                    break;

                default:
                    throw new NotImplementedException();
            }

            return state;
        }
        #endregion

    }
}
