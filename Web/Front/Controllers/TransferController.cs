using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using DFramework;
using Dotpay.Command;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Dotpay.Front.ViewModel;
using Dotpay.Front.YiSheng;
using Dotpay.FrontQueryService;

namespace Dotpay.Front.Controllers
{
    public class TransferController : BaseController
    {
        #region Transfer Page
        #region Internal transfer page
        [Route("~/transfer")]
        [Route("~/transfer/index")]
        public ActionResult Dotpay()
        {
            if (this.CurrentUser.IdentityInfo == null) return Redirect("/profile/identityverify?source=transfer");
            if (!this.CurrentUser.IsInitPaymentPassword) return Redirect("/profile/setpaymentpassword?source=transfer");
            return View();
        }
        #endregion

        #region Alipay Transfer Page
        [Route("~/transfer/alipay")]
        public ActionResult Alipay()
        {
            if (this.CurrentUser.IdentityInfo == null) return Redirect("/profile/identityverify?source=transfer");
            if (!this.CurrentUser.IsInitPaymentPassword) return Redirect("/profile/setpaymentpassword?source=transfer");
            return View();
        }
        #endregion

        #region Bank Transfer Page
        [Route("~/transfer/bank")]
        public ActionResult Bank()
        {
            if (this.CurrentUser.IdentityInfo == null) return Redirect("/profile/identityverify?source=transfer");
            if (!this.CurrentUser.IsInitPaymentPassword) return Redirect("/profile/setpaymentpassword?source=transfer");
            return View();
        }
        #endregion

        #region Ripple Transfer Page
        [Route("~/transfer/ripple")]
        public ActionResult Ripple()
        {
            if (this.CurrentUser.IdentityInfo == null) return Redirect("/profile/identityverify?source=transfer");
            if (!this.CurrentUser.IsInitPaymentPassword) return Redirect("/profile/setpaymentpassword?source=transfer");
            return View();
        }
        #endregion
        #endregion

        #region ValidateAccount
        [Route("~/transfer/validateaccount")]
        public async Task<ActionResult> ValidateAccount(string receiverAccount)
        {
            receiverAccount = receiverAccount.NullSafe().Trim();
            if (receiverAccount == this.CurrentUser.Email
                || receiverAccount == this.CurrentUser.LoginName)
            {
                return Json(new { valid = false, message = this.Lang("cannotTransferToYourSelf") });
            }

            var query = IoC.Resolve<IUserQuery>();
            UserIdentity userIdentity;
            if (receiverAccount.IsEmail())
            {
                userIdentity = await query.GetUserByEmail(receiverAccount.Trim());
            }
            //else if (user.LoginName.IsMobile()) { }//以后用户支持手机号注册
            else
            {
                userIdentity = await query.GetUserByLoginName(receiverAccount.Trim());
            }

            if (userIdentity != null && userIdentity.IsActive)
                return
                    Json(
                        new
                        {
                            valid = true,
                            account = receiverAccount.NullSafe().Trim(),
                            realName = userIdentity.IdentityInfo != null ? userIdentity.IdentityInfo.FullName : "",
                            message = userIdentity.IdentityInfo == null ? this.Lang("transferReceiverUnauth") : ""
                        });
            else
                return Json(new { valid = false, message = this.Lang("Invalid receiver account") });
        }

        #endregion

        #region Transfer Confirm Page
        [Route("~/transfer/{type}/confirm")]
        public async Task<ActionResult> CommonConfirm(string type, Guid txid)
        {
            var transferTransaction = Cache.Get<TransferTransactionSubmitViewModel>(txid.ToString());

            if (transferTransaction == null)
            {
                var query = IoC.Resolve<ITransactionQuery>();
                transferTransaction = await query.GetTransferTransactionSubmitDetailByTxid(txid);

                if (transferTransaction != null) Cache.Add(txid.ToString(), transferTransaction, TimeSpan.FromMinutes(20));
            }

            if (transferTransaction != null &&
                transferTransaction.TransferUserId == this.CurrentUser.UserId)
            {
                ViewBag.TransferTransaction = transferTransaction;
            }
            type = type.Substring(0, 1).ToUpper() + type.Substring(1);
            return View(type + "Confirm");
        }

        #endregion

        #region Transfer Result Page
        [Route("~/transfer/{type}/result")]
        public ActionResult InternalResult(string type, string txid)
        {
            var transferTransaction = Cache.Get<TransferTransactionSubmitViewModel>(type + txid);

            if (transferTransaction != null &&
                transferTransaction.TransferUserId == this.CurrentUser.UserId)
            {
                ViewBag.TransferTransaction = transferTransaction;
            }
            type = type.Substring(0, 1).ToUpper() + type.Substring(1);
            return View(type + "Result");
        }
        #endregion

        #region 预提交

        #region 转账到点付-预提交

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/transfer/dotpay/submit")]
        public async Task<ActionResult> DotpayTransferSubmit(string receiverAccount, decimal transferAmount, string memo, string realName)
        {
            var result = DotpayJsonResult.SystemError;
            receiverAccount = receiverAccount.NullSafe().Trim();
            var query = IoC.Resolve<IUserQuery>();
            UserIdentity userIdentity;
            if (receiverAccount.IsEmail())
            {
                userIdentity = await query.GetUserByEmail(receiverAccount);
            }
            //else if (user.LoginName.IsMobile()) { }//以后用户支持手机号注册
            else
            {
                userIdentity = await query.GetUserByLoginName(receiverAccount);
            }

            if (userIdentity == null) result = DotpayJsonResult.CreateFailResult(this.Lang("Invalid receiver account"));
            else if (transferAmount <= 0) result = DotpayJsonResult.CreateFailResult(this.Lang("Invalid amount"));
            else
            {
                try
                {
                    var txId = Guid.NewGuid();
                    var tx = new TransferTransactionSubmitViewModel()
                    {
                        TransferUserId = this.CurrentUser.UserId,
                        TransferTransactionId = txId,
                        Destination = receiverAccount,
                        Currency = CurrencyType.Cny,
                        Amount = transferAmount,
                        Payway = Payway.Dotpay,
                        DestinationAccountId = userIdentity.AccountId,
                        RealName = realName,
                        Memo = memo
                    };
                    Cache.Add(txId.ToString(), tx, TimeSpan.FromMinutes(20));
                    result = DotpayJsonResult<string>.CreateSuccessResult(txId.ToString());
                }
                catch (Exception ex)
                {
                    Log.Error("DotpayTransferSubmit Excepiton", ex);
                }
            }

            return Json(result);
        }
        #endregion

        #region 转账到支付宝-预提交

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/transfer/alipay/submit")]
        public ActionResult AlipayTransferSubmit(string receiverAccount, decimal transferAmount, string memo,
            string realName)
        {
            var result = DotpayJsonResult.SystemError;
            receiverAccount = receiverAccount.NullSafe().Trim();
            var query = IoC.Resolve<IUserQuery>();

            Regex regex = new Regex(@"^1[3|4|5|7|8][0-9]\d{4,8}$");

            if (!receiverAccount.IsEmail() && !regex.IsMatch(receiverAccount)) result = DotpayJsonResult.CreateFailResult(this.Lang("invalidAlipayAccount"));
            else if (transferAmount <= 0) result = DotpayJsonResult.CreateFailResult(this.Lang("Invalid amount"));
            else
            {
                try
                {
                    var txId = Guid.NewGuid();
                    var tx = new TransferTransactionSubmitViewModel()
                    {
                        TransferUserId = this.CurrentUser.UserId,
                        TransferTransactionId = txId,
                        Destination = receiverAccount,
                        Currency = CurrencyType.Cny,
                        Amount = transferAmount,
                        Payway = Payway.Alipay,
                        RealName = realName,
                        Memo = memo
                    };
                    Cache.Add(txId.ToString(), tx, TimeSpan.FromMinutes(20));
                    result = DotpayJsonResult<string>.CreateSuccessResult(txId.ToString());
                }
                catch (Exception ex)
                {
                    Log.Error("AlipayDepositSubmit Excepiton", ex);
                }
            }

            return Json(result);
        }
        #endregion

        #region 转账到银行-预提交

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/transfer/bank/submit")]
        public ActionResult BankTransferSubmit(Bank bank, string receiverAccount, decimal transferAmount, string memo,
            string realName)
        {
            var result = DotpayJsonResult.SystemError;
            receiverAccount = receiverAccount.NullSafe().Trim();
            var query = IoC.Resolve<IUserQuery>();

            Regex regex = new Regex(@"^\d{16}|\d{19}$");

            if (!regex.IsMatch(receiverAccount)) result = DotpayJsonResult.CreateFailResult(this.Lang("invalidBankAccount"));
            else if (transferAmount <= 0) result = DotpayJsonResult.CreateFailResult(this.Lang("Invalid amount"));
            else
            {
                try
                {
                    var txId = Guid.NewGuid();
                    var tx = new TransferTransactionSubmitViewModel()
                    {
                        TransferUserId = this.CurrentUser.UserId,
                        TransferTransactionId = txId,
                        Destination = receiverAccount,
                        Currency = CurrencyType.Cny,
                        Amount = transferAmount,
                        Bank = bank,
                        Payway = Payway.Bank,
                        RealName = realName,
                        Memo = memo
                    };
                    Cache.Add(txId.ToString(), tx, TimeSpan.FromMinutes(20));
                    result = DotpayJsonResult<string>.CreateSuccessResult(txId.ToString());
                }
                catch (Exception ex)
                {
                    Log.Error("BankTransferSubmit Excepiton", ex);
                }
            }

            return Json(result);
        }
        #endregion

        #region 转账到ripple-预提交

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/transfer/ripple/submit")]
        public ActionResult RippleTransferSubmit(string receiverAccount, decimal transferAmount, string memo)
        {
            var result = DotpayJsonResult.SystemError;
            receiverAccount = receiverAccount.NullSafe().Trim();
            var query = IoC.Resolve<IUserQuery>();

            Regex regex = new Regex(@"^r[a-zA-Z0-9]{32,33}$");

            if (!regex.IsMatch(receiverAccount)) result = DotpayJsonResult.CreateFailResult(this.Lang("invalidRippleAccount"));
            else if (transferAmount <= 0) result = DotpayJsonResult.CreateFailResult(this.Lang("Invalid amount"));
            else
            {
                try
                {
                    var txId = Guid.NewGuid();
                    var tx = new TransferTransactionSubmitViewModel()
                    {
                        TransferUserId = this.CurrentUser.UserId,
                        TransferTransactionId = txId,
                        Destination = receiverAccount,
                        Currency = CurrencyType.Cny,
                        Amount = transferAmount,
                        Payway = Payway.Bank,
                        Memo = memo
                    };
                    Cache.Add(txId.ToString(), tx, TimeSpan.FromMinutes(20));
                    result = DotpayJsonResult<string>.CreateSuccessResult(txId.ToString());
                }
                catch (Exception ex)
                {
                    Log.Error("RippleTransferSubmit Excepiton", ex);
                }
            }

            return Json(result);
        }
        #endregion
        #endregion

        #region 确认提交

        [HttpPost]
        [Route("~/transfer/{type}/confirm")]
        public async Task<ActionResult> DotpayTransferConfirm(string type, Guid txid, string paymentPassword)
        {
            var result = DotpayJsonResult.SystemError;

            try
            {
                var tx = Cache.Get<TransferTransactionSubmitViewModel>(txid.ToString());

                if (tx != null)
                {
                    dynamic cmd = null;
                    if (type.Equals("dotpay", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd = new SubmitTransferToDotpayTransactionCommand(txid, this.CurrentUser.AccountId,
                          tx.DestinationAccountId, CurrencyType.Cny, tx.Amount, tx.Memo, tx.RealName, paymentPassword);
                        await this.CommandBus.SendAsync(cmd);
                    }
                    else if (type.Equals("alipay", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd = new SubmitTransferToAlipayTransactionCommand(txid, this.CurrentUser.AccountId,
                          tx.Destination, Payway.Alipay, CurrencyType.Cny, tx.Amount, tx.Memo, tx.RealName, paymentPassword);
                        await this.CommandBus.SendAsync(cmd);
                    }
                    else if (type.Equals("bank", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd = new SubmitTransferToBankTransactionCommand(txid, this.CurrentUser.AccountId,
                          tx.Destination, tx.RealName, tx.Bank.Value, CurrencyType.Cny, tx.Amount, tx.Memo, paymentPassword);
                        await this.CommandBus.SendAsync(cmd);
                    }
                    else if (type.Equals("ripple", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd = new SubmitTransferToRippleTransactionCommand(txid, this.CurrentUser.AccountId,
                          tx.Destination, CurrencyType.Cny, tx.Amount, tx.Memo, paymentPassword);
                        await this.CommandBus.SendAsync(cmd);
                    }

                    if (cmd.CommandResult == ErrorCode.None)
                    {
                        var newId = Guid.NewGuid().Shrink().ToLower();
                        Cache.Add(type + newId, tx, TimeSpan.FromMinutes(5));
                        Cache.Remove(txid.ToString());
                        result = DotpayJsonResult<string>.CreateSuccessResult(newId);
                    }
                    else if (cmd.CommandResult == ErrorCode.AccountBalanceNotEnough)
                        result = DotpayJsonResult.CreateFailResult(-2, this.Lang("transferBalanceNotEnough"));
                    else if (cmd.CommandResult == ErrorCode.PaymentPasswordError)
                        result = DotpayJsonResult.CreateFailResult(-3, this.Lang("transferPaymentPasswordError"));
                    else if (cmd.CommandResult == ErrorCode.PaymentPasswordNotInitialized)
                        result = DotpayJsonResult.CreateFailResult(-4, this.Lang("transferPaymentPasswordNotInit"));
                    else if (cmd.CommandResult == ErrorCode.ExceedMaxPaymentPasswordFailTime)
                        result = DotpayJsonResult.CreateFailResult(-5, this.Lang("transferPaymentPasswordMaxErrorTime"));
                }

            }
            catch (Exception ex)
            {
                Log.Error("AlipayDepositSubmit Excepiton", ex);
            }


            return Json(result);
        }
        #endregion
    }
}