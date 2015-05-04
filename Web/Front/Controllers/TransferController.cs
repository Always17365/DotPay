using System;
using System.Collections.Generic;
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
        #region Internal Transfer Page
        [Route("~/transfer")]
        [Route("~/transfer/index")]
        public ActionResult Internal()
        {
            if (this.CurrentUser.IdentityInfo==null) return Redirect("/profile/identityverify?source=transfer");
            if (!this.CurrentUser.IsInitPaymentPassword) return Redirect("/profile/setpaymentpassword?source=transfer");
            return View();
        }
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

            if (userIdentity != null&&userIdentity.IsActive)
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
        [Route("~/transfer/dotpay/confirm")]
        public async Task<ActionResult> InternalConfirm(Guid txid)
        {
            var transferTransaction = Cache.Get<TransferTransactionSubmitViewModel>(txid.ToString());

            if (transferTransaction != null &&
                transferTransaction.TransferUserId == this.CurrentUser.UserId)
            {
                ViewBag.TransferTransaction = transferTransaction;
            }
            return View();
        }

        [Route("~/transfer/tpp/confirm")]
        public async Task<ActionResult> TppConfirm(string targetAccount, Payway payway, string realname, decimal amount)
        {
            return View();
        }

        [Route("~/transfer/bank/confirm")]
        public async Task<ActionResult> BankConfirm(string targetAccount, Bank bank, string realname, decimal amount)
        {
            return View();
        }
        [Route("~/transfer/ripple/confirm")]
        public async Task<ActionResult> RippleConfirm(string rippleAddress, CurrencyType currency, decimal amount)
        {
            return View();
        }
        #endregion

        #region Transfer Result Page
        [Route("~/transfer/dotpay/result")]
        public async Task<ActionResult> InternalResult(string txid)
        {
            var transferTransaction = Cache.Get<TransferTransactionSubmitViewModel>(txid);

            if (transferTransaction != null &&
                transferTransaction.TransferUserId == this.CurrentUser.UserId)
            {
                ViewBag.TransferTransaction = transferTransaction;
            }
            return View();
        }

        [Route("~/transfer/tpp/result")]
        public async Task<ActionResult> TppResult(string targetAccount, Payway payway, string realname, decimal amount)
        {
            return View();
        }

        [Route("~/transfer/bank/result")]
        public async Task<ActionResult> BankResult(string targetAccount, Bank bank, string realname, decimal amount)
        {
            return View();
        }
        [Route("~/transfer/ripple/result")]
        public async Task<ActionResult> RippleResult(string rippleAddress, CurrencyType currency, decimal amount)
        {
            return View();
        }
        #endregion

        #region 预提交

        #region 转账到点付-预提交

        [HttpPost]
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
                    //var cmd = new CreateTransferToDotpayTransactionCommand(this.CurrentUser.AccountId,
                    //    userIdentity.AccountId, CurrencyType.Cny, transferAmount,
                    //    memo, string.Empty);
                    //await this.CommandBus.SendAsync(cmd);

                    //if (cmd.CommandResult.Item1 == ErrorCode.None)
                    //    result = DotpayJsonResult<string>.CreateSuccessResult(cmd.CommandResult.Item2);
                    //else if (cmd.CommandResult.Item1 == ErrorCode.AccountBalanceNotEnough)
                    //    result = DotpayJsonResult.CreateFailResult(this.Lang("transferBalanceNotEnough"));

                }
                catch (Exception ex)
                {
                    Log.Error("AlipayDepositSubmit Excepiton", ex);
                }
            }

            return Json(result);
        }
        #endregion
        #endregion



        #region 确认提交

        #region 转账到点付-确认提交

        [HttpPost]
        [Route("~/transfer/dotpay/confirm")]
        public async Task<ActionResult> DotpayTransferConfirm(Guid txid, string paymentPassword)
        {
            var result = DotpayJsonResult.SystemError;

            try
            {
                var tx = Cache.Get<TransferTransactionSubmitViewModel>(txid.ToString());

                if (tx != null)
                {
                    var cmd = new SubmitTransferToDotpayTransactionCommand(txid, this.CurrentUser.AccountId,
                        tx.DestinationAccountId, CurrencyType.Cny, tx.Amount, tx.Memo, tx.RealName, paymentPassword);
                    await this.CommandBus.SendAsync(cmd);

                    if (cmd.CommandResult == ErrorCode.None)
                        result = DotpayJsonResult<string>.CreateSuccessResult(txid.ToString());
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
        #endregion
    }
}