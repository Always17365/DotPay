using FC.Framework;
using DotPay.Common;
using DotPay.Command;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.QueryService;
using QConnectSDK.Context;
using QConnectSDK;
using System.IO;

namespace DotPay.Web.Controllers
{
    public class DepositController : BaseController
    {
       
        [Route("~/deposits/{currency}")]
        public ActionResult Deposits(CurrencyType currency)
        {
            ViewBag.Currency = this.Lang(currency.GetDescription());
            ViewBag.CurrencyCode = currency.ToString();
            string viewName = string.Empty;
            #region 获取充值地址
            if (currency == CurrencyType.CNY) viewName = "CNYDeposits";
            else  viewName = "VirtualCoinDeposits";
            #endregion

            return View(viewName);
        }


        #region Post
 
        #region 生成充值地址
        [Route("~/deposit/generateNewAddress")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateNewAddress(CurrencyType currency)
        {
            var result = FCJsonResult.CreateFailResult(this.Lang("Unknow Exception,Please refresh the page and try again"));
            var cacheKey = CacheKey.USER_GENERATE_PAYMENT_ADDRESS + currency.ToString() + this.CurrentUser.UserID;
            object tmp;
            if (Config.Debug || !Cache.TryGet(cacheKey, out tmp))
            {
                try
                {
                    var cmd = new GeneratePaymentAddress(this.CurrentUser.UserID, currency);
                    this.CommandBus.Send(cmd);
                    Cache.Add(cacheKey, cmd.ID);
                    result = FCJsonResult.CreateSuccessResult(this.Lang("Generating deposit address ..."));
                }
                catch (Exception ex)
                {
                    Log.Error("Action GenerateNewAddress Error", ex);
                }
            }
            return Json(result);
        }
        #endregion

             #endregion

        #region 私有方法
        private CurrencyType GetCurrencyTypeByDepositCode(string code)
        {
            var currencyStr = code.Substring(0, 3);
            var currency = default(CurrencyType);
            var currencies = Enum.GetValues(typeof(CurrencyType));

            foreach (var value in currencies)
            {
                if (value.ToString().Substring(0, 3).Equals(currencyStr, StringComparison.OrdinalIgnoreCase))
                    currency = (CurrencyType)value;
            }

            return currency;
        }
        #endregion

        #region 查询我的充值记录
        [AllowAnonymous]
        [Route("~/action/getdeposits")]
        public ActionResult GenerateStatisticsForInterval(int start, int limit, CurrencyType currencyType)
        {
             int totalCount = 0;
             IEnumerable<DepositInListModel> result = default(IEnumerable<DepositInListModel>);
            if (currencyType == CurrencyType.CNY)
            {
                 totalCount = IoC.Resolve<IDepositQuery>().CountCNYDepositByUserID(this.CurrentUser.UserID);
                 result = IoC.Resolve<IDepositQuery>().GetCNYDepositByUserID(this.CurrentUser.UserID, start, limit);
            } else {
                totalCount = IoC.Resolve<IDepositQuery>().CountVirtualCoinDepositByUserID(this.CurrentUser.UserID, currencyType);
                result = IoC.Resolve<IDepositQuery>().GetVirtualCoinDepositByUserID(this.CurrentUser.UserID, currencyType, start, limit);   
            }
            return Json(new { data = result, Code=1, totalCount = totalCount}, JsonRequestBehavior.AllowGet);
        }
     

        #endregion
    }
}
