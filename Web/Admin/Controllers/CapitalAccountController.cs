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
    public class CapitalAccountController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetCapitalAccountCountBySearch(string name)
        {
            var count = IoC.Resolve<ICapitalAccountQuery>().CountCapitalAccountBySearch( name.NullSafe());

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetCapitalAccountBySearch(string name, int page)
        {
            var currencies = IoC.Resolve<ICapitalAccountQuery>().GetCapitalAccountBySearch(name.NullSafe(), page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }

        [HttpPost]
        public ActionResult Create(string BankAccount, string OwnerName, Bank Bank)
        {
            
            try
            {
                var cmd = new CreateCapitalAccount( BankAccount,  OwnerName,  Bank, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }

        public ActionResult GetCapitalAccountType()
        {
            return Json(this.GetSelectList<Bank>(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Enable(int capitalAccountID)
        {
            var cmd = new EnableCapitalAccount(capitalAccountID, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        [HttpPost]
        public ActionResult Disable(int capitalAccountID)
        {
            var cmd = new DisableCapitalAccount(capitalAccountID, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

    }
}
