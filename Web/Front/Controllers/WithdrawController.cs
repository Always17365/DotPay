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
    public class WithdrawController : BaseController
    {
        #region view
        [Route("~/withdraw/{currency}")]
        public ActionResult Index(CurrencyType currency)
        {
            var viewName = string.Empty;
            if (currency == CurrencyType.CNY) viewName = "CNY";
            else viewName = "VirtualCoin";
            ViewBag.VirtualCoinName = currency.ToString();
            var currentUserID = this.CurrentUser.UserID;
            var account = IoC.Resolve<IAccountQuery>().GetAccountByUserID(currentUserID, currency);

            ViewBag.Balance = (account != null && account.ID > 0) ? account.Balance : 0;
            ViewBag.CurrentDayLimit = GetUserWithdrawDayLimitRemain(currency);
            ViewBag.CurrencySetting = IoC.Resolve<ICurrencyQuery>().GetCurrency(currency);
            ViewBag.Currency = currency;
            ViewBag.TodayLimit = this.GetUserWithdrawDayLimitRemain(currency);
            return View(viewName);
        }
        [Route("~/withdrawals/{currency}")]
        public ActionResult Withdraw(CurrencyType currency)
        {
            ViewBag.Currency = this.Lang(currency.GetDescription());
            ViewBag.CurrencyCode = currency.ToString();
            string viewName = string.Empty;
            #region 获取充值地址
            if (currency == CurrencyType.CNY) viewName = "CNYWithdraw";
            else viewName = "VirtualCoinWithdraw";
            #endregion

            return View(viewName);
        }

        #endregion

        #region 人民币提现
        [Route("~/withdraw/cny")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CNYWithdraw(bool withdrawToCode, int? accountID, decimal amount, Bank bank, string bankAccount,
                             string tradePassword, string sms_otp, string ga_otp)
        {
            //if (!CheckUserIsPassRealNameAuthAndEmailIsVerify())
            //{
            //    return Json(new FCJsonResult(-1));
            //}
            var result = default(FCJsonResult);
            var currencySetting = IoC.Resolve<ICurrencyQuery>().GetCurrency(CurrencyType.CNY);

            if (!this.CurrentUser.IsVerifyEmail) result = FCJsonResult.CreateFailResult(this.Lang("Please verify your email before withdraw."));
            else if (string.IsNullOrEmpty(this.CurrentUser.IdNo)) result = FCJsonResult.CreateFailResult(this.Lang("Please update your identity informations before withdraw."));

            else if (!Config.Debug && this.CurrentUser.IsOpenTwoFactorGA && !this.VerifyUserGA(ga_otp))
                result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Your Google Authenticator code error."));
            else if (!Config.Debug && this.CurrentUser.IsOpenTwoFactorSMS && !this.VerifyUserSms(sms_otp))
                result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Your Sms Authenticator code error."));
            else if (string.IsNullOrEmpty(tradePassword))
                result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Your trade passowrd error."));
            else
            {
                try
                {
                    if (!withdrawToCode)
                    {
                        var cmd = new SubmitCNYWithdraw(accountID, this.CurrentUser.UserID, amount, bank, bankAccount.Trim(), tradePassword);
                        this.CommandBus.Send(cmd);
                        this.UpdateUserWithdrawDayLimitRemain(amount, CurrencyType.CNY);
                    }
                    else
                    {
                        var cmd = new WithdrawToDepositCode(CurrencyType.CNY, this.CurrentUser.UserID, amount, tradePassword);
                        this.CommandBus.Send(cmd);
                        this.UpdateUserWithdrawDayLimitRemain(amount, CurrencyType.CNY);
                        return Json(new { Code = 2, Message = this.Lang("Withdraw to DotPay Deposit Code successfully."), DepositCode = cmd.CommandResult });
                    }
                }
                catch (CommandExecutionException ex)
                {
                    if (ex.ErrorCode == (int)ErrorCode.TradePasswordError)
                        result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Your trade passowrd error."));
                    else if (ex.ErrorCode == (int)ErrorCode.AccountBalanceNotEnough)
                        result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Your balance is not enough."));
                    else if (ex.ErrorCode == (int)ErrorCode.WithdrawAmountOutOfRange)
                        result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Request amount out of range."));
                    else Log.Error("Action VirtualCoinWithdraw Error", ex);
                }
            }
            return Json(result);
        }
        #endregion

        #region 虚拟币提现

        [Route("~/withdraw/v/{currency}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VirtualCoinWithdraw(bool withdrawToCode, CurrencyType currency, string address, decimal amount,
                                                string tradePassword, string sms_otp, string ga_otp)
        {
            var currencySetting = IoC.Resolve<ICurrencyQuery>().GetCurrency(currency);
            var result = FCJsonResult.CreateFailResult(this.Lang("Unknow Exception,Please refresh the page and try again"));
            if (!this.CurrentUser.IsVerifyEmail) result = FCJsonResult.CreateFailResult(this.Lang("Please verify your email before withdraw."));
            else if (!Config.Debug && this.CurrentUser.IsOpenTwoFactorSMS && !this.VerifyUserGA(sms_otp))
                result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Your Google Authenticator code error."));
            else if (!Config.Debug && this.CurrentUser.IsOpenTwoFactorGA && !this.VerifyUserSms(ga_otp))
                result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Your Sms Authenticator code error."));
            else if (this.GetUserWithdrawDayLimitRemain(currency) < amount)
                result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. You lack of withdrawal limit today."));
            else if (string.IsNullOrEmpty(tradePassword))
                result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Your trade passowrd error."));
            else
                try
                {
                    if (!withdrawToCode)
                    {
                        var otherUserID = 0;
                       
                            var cmd = new SubmitVirtualCoinWithdraw(this.CurrentUser.UserID, currency, amount, address.Trim(), tradePassword);
                            this.CommandBus.Send(cmd);
                            result = FCJsonResult.CreateSuccessResult(this.Lang("Submit successfuly."));
                    }
                    else
                    {
                        var cmd = new WithdrawToDepositCode(currency, this.CurrentUser.UserID, amount, tradePassword);
                        this.CommandBus.Send(cmd);
                        return Json(new { Code = 2, Message = this.Lang("Withdraw to DotPay Deposit Code successfully."), DepositCode = cmd.CommandResult });
                    }
                }
                catch (CommandExecutionException ex)
                {
                    if (ex.ErrorCode == (int)ErrorCode.TradePasswordError)
                        result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Your trade passowrd error."));
                    else if (ex.ErrorCode == (int)ErrorCode.AccountBalanceNotEnough)
                        result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Your balance is not enough."));
                    else if (ex.ErrorCode == (int)ErrorCode.WithdrawAmountOutOfRange)
                        result = FCJsonResult.CreateFailResult(this.Lang("Unable to submit your request. Request amount out of range."));
                    else Log.Error("Action VirtualCoinWithdraw Error", ex);
                }

            return Json(result);
        }
        #endregion


        #region 城市数据

        [Route("~/area/province")]
        [HttpPost]
        public ActionResult Province()
        {
            var provinces = IoC.Resolve<ICityQuery>().GetAllProvince();

            return Json(provinces);
        }


        [Route("~/area/city")]
        [HttpPost]
        public ActionResult City(int provinceID)
        {
            var cities = IoC.Resolve<ICityQuery>().GetCityByProvinceID(provinceID);

            return Json(cities);
        }
        #endregion

        #region 银行网点数据

        [Route("~/bankOutlets")]
        [HttpPost]
        public ActionResult BankOutlets(int provinceID, int cityID, Bank bank)
        {
            var outlets = IoC.Resolve<IBankOutletsQuery>().GetBankoutletsByProvinceIDAndCityID(bank, provinceID, cityID);

            return Json(outlets);
        }
        #endregion

        #region 私有方法

        #region 加载用户的提现记录数据view bag
        private void LoadWithdrawRecord(CurrencyType currency, int page)
        {
            var query = IoC.Resolve<IWithdrawQuery>();
            var currentUserID = this.CurrentUser.UserID;
            //ViewBag.Records = query.GetLastTenVirtualCoinWithdrawByUserID(currentUserID, currency, page, Constants.DEFAULT_PAGE_COUNT);
        }
        #endregion


        #region 验证用户的谷歌身份验证码
        private bool VerifyUserGA(string otp)
        {
            var secret = IoC.Resolve<IUserQuery>().GetUserGoogleAuthenticationSecretByID(this.CurrentUser.UserID);

            return otp == Utilities.GenerateGoogleAuthOTP(secret);
        }
        #endregion

        #region 验证用户的短信验证码
        private bool VerifyUserSms(string otp)
        {
            try
            {
                var secretPair = IoC.Resolve<IUserQuery>().GetUserSmsSecretByID(this.CurrentUser.UserID);
                return otp == Utilities.GenerateSmsOTP(secretPair.Item1, secretPair.Item2);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 获取用户的日提现额度
        private decimal GetUserWithdrawDayLimitRemain(CurrencyType currency)
        {
            var limiteAmount = 0M;

            var cacheKey = CacheKey.USER_WITHDRAW_DAY_LIMITE + currency.ToString() + this.CurrentUser.UserID;
            var currencySetting = IoC.Resolve<ICurrencyQuery>().GetCurrency(currency);

            Cache.TryGet(cacheKey, out limiteAmount);

            return currencySetting.WithdrawDayLimit - limiteAmount;
        }
        #endregion

        #region 更新用户的日提现额度
        private void UpdateUserWithdrawDayLimitRemain(decimal withdrawAmount, CurrencyType currency)
        {
            var limiteAmount = 0M;

            var cacheKey = CacheKey.USER_WITHDRAW_DAY_LIMITE + currency.ToString() + this.CurrentUser.UserID;

            Cache.TryGet(cacheKey, out limiteAmount);

            limiteAmount += withdrawAmount;
            var now = DateTime.Now;
            Cache.Add(cacheKey, limiteAmount, new DateTime(now.Year, now.Month, now.Day, 23, 59, 56));
        }
        #endregion
        #endregion
        #region 
        [AllowAnonymous]
        [Route("~/action/getwithdraw")]
        public ActionResult GenerateStatisticsForInterval(int start, int limit, CurrencyType currencyType)
        {
            int totalCount = 0;
            IEnumerable<WithdrawListModel> result = default(IEnumerable<WithdrawListModel>);
            if (currencyType == CurrencyType.CNY)
            {
                totalCount = IoC.Resolve<IWithdrawQuery>().CountMyCoinWithdraw_Sql(this.CurrentUser.UserID,CurrencyType.CNY);
                result = IoC.Resolve<IWithdrawQuery>().GetMyCNYWithdraw(this.CurrentUser.UserID, start, limit);
            }
            else
            {
                totalCount = IoC.Resolve<IWithdrawQuery>().CountMyCoinWithdraw_Sql(this.CurrentUser.UserID, currencyType);
                result = IoC.Resolve<IWithdrawQuery>().GetMyVirtualCoinWithdraw(this.CurrentUser.UserID, currencyType, start, limit);
            }
            return Json(new { data = result, Code=1, totalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }
      
        #endregion
    }
}
