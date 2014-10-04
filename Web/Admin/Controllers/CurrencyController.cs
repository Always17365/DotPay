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
    public class CurrencyController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetCurrencyCountBySearch(string code, string name)
        {
            var count = IoC.Resolve<ICurrencyQuery>().GetCurrencyCountBySearch(code.NullSafe(), name.NullSafe());

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetCurrenciesBySearch(string code, string name, int page)
        {
            var currencies = IoC.Resolve<ICurrencyQuery>().GetCurrenciesBySearch(code.NullSafe(), name.NullSafe(), page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }

        [HttpPost]
        public ActionResult Create(CurrencyCreateModel currency)
        {
            currency.Code = Enum.Parse(typeof(CurrencyType), currency.CurrencyID.ToString()).ToString();
            var existResult = IoC.Resolve<ICurrencyQuery>().ExistCurrency(currency.Code, currency.Name);

            if (existResult.Item1) return Json(new JsonResult(-1));
            if (existResult.Item2) return Json(new JsonResult(-2));

            try
            {
                var cmd = new CreateCurrency(currency.CurrencyID, currency.Code, currency.Name, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }

        public ActionResult GetCurrencyType()
        {
            return Json(this.GetSelectList<CurrencyType>(CurrencyType.CNY), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Enable(int currencyID)
        {
            var cmd = new EnableCurrency(currencyID, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        [HttpPost]
        public ActionResult Disable(int currencyID)
        {
            var cmd = new DisableCurrency(currencyID, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }
        /************************************************************************************************************************************************/

        [HttpPost]
        public ActionResult ModifyCurrencyDepositFeeRate(int currencyID, decimal depositFixedFee, decimal depositFeeRate)
        {
            var cmd = new ModifyCurrencyDepositFeeRate(currencyID,depositFixedFee,depositFeeRate,this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        [HttpPost]
        public ActionResult ModifyCurrencyWithdrawFeeRate(int currencyID, decimal withdrawFixedFee, decimal withdrawFeeRate)
        {
            var cmd = new ModifyCurrencyWithdrawFeeRate(currencyID, withdrawFixedFee, withdrawFeeRate, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        [HttpPost]
        public ActionResult ModifyCurrencyWithdrawOnceLimit(int currencyID, decimal onceLimit)
        {
            var cmd = new ModifyCurrencyWithdrawOnceLimit(currencyID, onceLimit, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        [HttpPost]
        public ActionResult ModifyCurrencyWithdrawDayLimit(int currencyID, decimal dayLimit)
        {
            var cmd = new ModifyCurrencyWithdrawDayLimit(currencyID, dayLimit, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        [HttpPost]
        public ActionResult ModifyCurrencyWithdrawVerifyLine(int currencyID, decimal verifyLine)
        {
            var cmd = new ModifyCurrencyWithdrawVerifyLine(currencyID, verifyLine, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }
        
        [HttpPost]
        public ActionResult ModifyCurrencyWithdrawOnceMin(int currencyID, decimal WithdrawOnceMin)
        {
            var cmd = new ModifyCurrencyWithdrawOnceMin(currencyID, WithdrawOnceMin, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }
        [HttpPost]
        public ActionResult ModifyCurrencyNeedConfirm(int currencyID, int num)
        {
            var cmd = new ModifyCurrencyNeedConfirm(currencyID, num, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }
        
        
    }
}
