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

    public class BankOutletsController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult BankInfo()
        {
            return View();
        }      

        [HttpPost]
        public ActionResult GetBankInfoCountBySearch(int? bankType, int? province, int? city)
        {
            var count = IoC.Resolve<IBankOutletsQuery>().GetBankInfoCountBySearch(bankType, province, city);

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetBankInfoBySearch(int? bankType, int? province, int? city, int page)
        {
            var currencies = IoC.Resolve<IBankOutletsQuery>().GetBankInfoBySearch(bankType, province, city, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }

        [HttpPost]
        public ActionResult CreateBankOutlets(Bank bank, int provinceID, int cityID, string bankName)
        {
            try
            {
                var cmd = new CreateBankOutlets( bank,  provinceID,  cityID,  bankName, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }

        [HttpPost]
        public ActionResult RemoveBankOutlets(int bankOutletsId)
        {
            try
            {
                var cmd = new RemoveBankOutlets(bankOutletsId, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }

    }
}
