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
        #region view
        [Route("~/deposit/{currency}")]
        public ActionResult Index(CurrencyType currency)
        {
            ViewBag.Currency = this.Lang(currency.GetDescription());
            ViewBag.CurrencyCode = currency.ToString();
            var currentUserID = this.CurrentUser.UserID;
            var account = IoC.Resolve<IAccountQuery>().GetAccountByUserID(currentUserID, currency);
            var paymentAddress = string.Empty;
            var viewName = string.Empty;

            ViewBag.CurrencySettings = IoC.Resolve<ICurrencyQuery>().GetCurrency(currency);

            ViewBag.Balance = (account == null || account.ID == 0) ? 0 : account.Balance;

            #region 获取充值地址
            if (currency == CurrencyType.CNY) viewName = "CNY";
            else if (currency == CurrencyType.STR) viewName = "Stellar";
            else
            {
                viewName = "VirtualCoin";
                paymentAddress = IoC.Resolve<IPaymentAddressQuery>().GetPaymentAddressByUserID(currency, currentUserID);

                if (!string.IsNullOrEmpty(paymentAddress))
                {
                    var addressPair = paymentAddress.Split(Constants.DefaultSplitChars, StringSplitOptions.RemoveEmptyEntries);

                    ViewBag.PaymentAddress = addressPair[0];

                    if (currency == CurrencyType.NXT)
                    {
                        ViewBag.NxtNumericPaymentAddress = addressPair[1];
                        ViewBag.NxtPublicKey = addressPair[2];
                    }
                }
            }
            #endregion

            return View(viewName);
        }
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

        #endregion

        #region Post
 

        #region 提交恒星币充值
        [HttpPost]
        [Route("~/depositstellar")]
        public ActionResult StellarDeposit(decimal depositamount, string checkcode)
        {
            var result = FCJsonResult.CreateFailResult(this.Lang("Unknow Exception,Please refresh the page and try again"));
            if (!Config.Debug && !CheckImageCode(checkcode))
                result = FCJsonResult.CreateFailResult(this.Lang("Cpatcha  error."));

            if (depositamount > 0)
            {
                try
                {
                    var txid = "waiting user transfer";
                    var cmd = new CreateReceivePaymentTransaction(txid, "stellar", depositamount, CurrencyType.STR, this.CurrentUser.UserID);
                    this.CommandBus.Send(cmd);
                    return Json(new { Code = 1, Address = "ghT2PjssWmFJkS5UPicDDiReMyt6ZZ7xMH" });
                }
                catch (CommandExecutionException ex)
                {
                    Log.Error("Action depositstellar Error", ex);
                }
            }
            return Json(result);
        }
        #endregion

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

        #region 获取最新生成地址
        [Route("~/deposit/getDepositAddress")]
        [HttpPost]
        public ActionResult GetNewAddress(CurrencyType currency)
        {
            var paymentAddress = IoC.Resolve<IPaymentAddressQuery>().GetPaymentAddressByUserID(currency, this.CurrentUser.UserID);
            var addressPair = paymentAddress.Split(Constants.DefaultSplitChars, StringSplitOptions.RemoveEmptyEntries);

            if (currency == CurrencyType.NXT)
                return Json(new { PaymentAddress = addressPair[0], NxtNumericPaymentAddress = addressPair[1], NxtPublicKey = addressPair[2] });
            else
                return Json(new { PaymentAddress = addressPair[0] });
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
