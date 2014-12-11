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

namespace DotPay.Web.Controllers
{
    public class ForgetPasswordController : BaseController
    {
        #region views
        #region Forget password
        [AllowAnonymous]
        [Route("~/forgetPassword")]
        public ActionResult Index() { return View(); }
        #endregion

        #region Reset login password by two-factor
        [HttpGet]
        [Route("~/ResetLoginPasswordBy2FA/{hash}")]
        [AllowAnonymous]
        public ActionResult ResetLoginPasswordBy2FA(string hash)
        {
            var _hash = Session["resetLoginPwdHash"] == null ? string.Empty : Session["resetLoginPwdHash"].ToString();
            if (hash != _hash)
                ViewBag.Result = FCJsonResult.CreateFailResult(this.Lang("Invalidate link."));
            else
                ViewBag.Result = FCJsonResult.Success;

            if (Session["resetUserID"] != null)
            {
                var userID = (int)Session["resetUserID"];
                var user = IoC.Resolve<IUserQuery>().GetUserByID(userID);
                ViewBag.ResetUser = user;
            }
            return View();
        }
        #endregion

        #region Reset trade password by two-factor
        [HttpGet]
        [Route("~/resetPayPwdByMobile/{hash}")]
        [AllowAnonymous]
        public ActionResult ResetTradePasswordBy2FA(string hash)
        {
            var _hash = Session["resetTradePwdHash"] == null ? string.Empty : Session["resetTradePwdHash"].ToString();
            if (hash != _hash)
                ViewBag.Result = FCJsonResult.CreateFailResult(this.Lang("Invalidate link."));
            else
                ViewBag.Result = FCJsonResult.Success;

            return View();
        }
        #endregion

        #region Reset login password by email token
        [AllowAnonymous]
        [Route("~/resetPwd-{userID}-{token}")]
        public ActionResult ResetPasswordByEmailToken(int userID, string token)
        {
            ViewData["emailToken"] = token;
            ViewData["resetUserID"] = userID;
            var tokenObj = IoC.Resolve<ITokenQuery>().GetIdAndExpiredAtByTokenAndUserIDWhichIsNotUsed(token, userID, Common.TokenType.PasswordReset);

            if (tokenObj == null)
                ViewBag.ErrorMessage = this.Lang("Invalid password reset link.");
            else if (tokenObj.Item2.ToLocalDateTime().AddMinutes(30) < DateTime.Now)
                ViewBag.ErrorMessage = this.Lang("Reset login password link is expired.");
            else
            {
                var cmd = new TokenUse(tokenObj.Item1);
                this.CommandBus.Send(cmd);
                ViewBag.Code = 1;
            }

            return View();
        }
        #endregion
        #endregion

        #region Forget login password
        [Route("~/forgetPassword")]
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword(string email, string checkCode)
        {
            var result = FCJsonResult.UnknowFail;
            var cacheKey = CacheKey.USER_RESEND_ACTIVE_EMAIL + email.ToLower();

            if (!email.NullSafe().Trim().IsEmail())
            {
                return Json(FCJsonResult.CreateFailResult(this.Lang("Unable to locate an account with that email address")));
            }

            if (!Config.Debug)
            {
                DateTime lastSendEmailTime;
                if (Cache.TryGet<DateTime>(cacheKey, out lastSendEmailTime))
                {
                    if (lastSendEmailTime.AddMinutes(15) > DateTime.Now)
                        return Json(FCJsonResult.CreateFailResult(this.Lang("Send email too frequently, please try again 15 minutes later.")));
                }
            }

            var user = IoC.Resolve<IUserQuery>().GetUserByEmail(email.NullSafe().Trim());

            if (!this.CheckImageCode(checkCode, CaptchaType.ForgetPassword)) result = FCJsonResult.CreateFailResult(this.Lang("Cpatcha  error."));
            else if (user == null) return Json(FCJsonResult.CreateFailResult(this.Lang("Unable to locate an account with that email address")));
            else if (user.IsBindGA || !string.IsNullOrEmpty(user.Mobile))
            {
                var hash = Guid.NewGuid().Shrink();
                Session["resetLoginPwdHash"] = hash;
                Session["resetUserID"] = user.UserID;
                ViewData["resetPwdHash"] = hash;
                this.KeepCurrentUserInfoInTmpAndReturnHash(user);
                return Json(new { Code = 2, Hash = hash });
            }
            else
            {
                try
                {
                    var cmd = new UserForgetPassword(user.UserID);
                    this.CommandBus.Send(cmd);

                    Cache.Add(cacheKey, DateTime.Now);
                    result = FCJsonResult.CreateSuccessResult(this.Lang("Please confirm the password reset via email."));
                }
                catch (CommandExecutionException ex)
                {
                    Cache.Remove(cacheKey);
                    Log.Error("Action forgetPassword Error", ex);
                }
            }
            return Json(result);
        }
        #endregion

        #region Forget trade password
        [Route("~/forgetPayPassword")]
        [AllowAnonymous]
        [HttpPost] 
        public ActionResult ForgetTradePassword()
        {
            var result = FCJsonResult.UnknowFail;

            if (/*this.CurrentUser.IsBindGA ||*/ !string.IsNullOrEmpty(this.CurrentUser.Mobile))
            {
                var hash = Guid.NewGuid().Shrink();
                Session["resetTradePwdHash"] = hash;
                ViewData["resetTradePwdHash"] = hash;
                return Json(new { Code = 2, Hash = hash });
            }

            else if (this.CurrentUser.IsVerifyEmail)
            {
                var cacheKey = CacheKey.USER_RESEND_ACTIVE_EMAIL + this.CurrentUser.Email.ToLower();

                if (!Config.Debug)
                {
                    DateTime lastSendEmailTime;
                    if (Cache.TryGet<DateTime>(cacheKey, out lastSendEmailTime))
                    {
                        if (lastSendEmailTime.AddMinutes(15) > DateTime.Now)
                            return Json(FCJsonResult.CreateFailResult(this.Lang("Send email too frequently, please try again 15 minutes later.")));
                    }
                }

                try
                {
                    var cmd = new UserForgetPassword(this.CurrentUser.UserID);
                    this.CommandBus.Send(cmd);

                    Cache.Add(cacheKey, DateTime.Now);
                    result = FCJsonResult.CreateSuccessResult(this.Lang("Please confirm the trade password reset via email."));
                }
                catch (CommandExecutionException ex)
                {
                    Cache.Remove(cacheKey);
                    Log.Error("Action forgetPassword Error", ex);
                }
            }
            else
                result = FCJsonResult.CreateFailResult(this.Lang("Please verify your email or enable Google/Sms Authenticator first."));
            return Json(result);
        }
        #endregion

        #region Reset login password by email token
        [Route("~/resetpassword")]
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPwdByEmailToken(int userID, string token, string password, string confirmpassword)
        {
            var result = FCJsonResult.CreateFailResult(this.Lang("Unknow Exception,Please refresh the page and try again"));
            var tokenObj = IoC.Resolve<ITokenQuery>().GetIdAndExpiredAtByTokenAndUserIDWhichIsNotUsed(token, userID, Common.TokenType.PasswordReset);
            if (password == confirmpassword && password.NullSafe().Length >= 6)
            {

                var user = IoC.Resolve<IUserQuery>().GetUserGoogleAuthenticationSecretByID(userID);
                var cmd = new UserResetPasswordByEmailToken(userID, password, token);
                try
                {
                    this.CommandBus.Send(cmd);
                    result = FCJsonResult.CreateSuccessResult(this.Lang("Reset login password successfully."));
                }
                catch (CommandExecutionException ex)
                {
                    Log.Error("Action ResetPwdByEmailToken Error", ex);
                }
            }

            return Json(result);
        }

        #endregion

        #region Reset login password by two-factor
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [Route("~/resetLoginPwdBy2TF")]
        public ActionResult ResetLoginPasswordByTwoFactor(string password, string confirmpassword, string ga_Otp, string sms_Otp)
        {
            var result = FCJsonResult.UnknowFail;
            if (Session["resetUserID"] != null)
            {
                var userID = (int)Session["resetUserID"];
                if (password == confirmpassword)
                {
                    try
                    {
                        var cmd = new UserResetPasswordByTwoFactor(userID, password, ga_Otp, sms_Otp);
                        this.CommandBus.Send(cmd);
                        Session.Remove("resetLoginPwdHash");
                        result = FCJsonResult.CreateSuccessResult(this.Lang("Reset your login password successfully"));
                    }
                    catch (CommandExecutionException ex)
                    {
                        if (ex.ErrorCode == (int)ErrorCode.GAPasswordError)
                            result = FCJsonResult.CreateFailResult(this.Lang("Unable to update your login password. Your Google Authenticator code error."));
                        else if (ex.ErrorCode == (int)ErrorCode.SMSPasswordError)
                            result = FCJsonResult.CreateFailResult(this.Lang("Unable to update your login password. Your Sms Authenticator code error."));
                        else
                            Log.Error("Action ResetLoginPasswordByTwoFactor Error", ex);
                    }
                }
            }

            return Json(result);
        }
        #endregion


        #region Reset trade password by email token
        [Route("~/resetTradePassword")]
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetTradePwdByEmailToken(int userID, string token, string password, string confirmpassword)
        {
            var result = FCJsonResult.CreateFailResult(this.Lang("Unknow Exception,Please refresh the page and try again"));
            var tokenObj = IoC.Resolve<ITokenQuery>().GetIdAndExpiredAtByTokenAndUserIDWhichIsNotUsed(token, userID, Common.TokenType.PasswordReset);
            if (password == confirmpassword && password.NullSafe().Length >= 6)
            {

                var user = IoC.Resolve<IUserQuery>().GetUserGoogleAuthenticationSecretByID(userID);
                var cmd = new UserResetTradePasswordByEmailToken(userID, password, token);
                try
                {
                    this.CommandBus.Send(cmd);
                    result = FCJsonResult.CreateSuccessResult(this.Lang("Reset trade password successfully."));
                }
                catch (CommandExecutionException ex)
                {
                    Log.Error("Action ResetTradePwdByEmailToken Error", ex);
                }
            }

            return Json(result);
        }

        #endregion

        #region Reset trade password by two-factor
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/resetPayPwdByMobile")]
        public ActionResult ResetTradePasswordByTwoFactor(string paypassword, string confirmpassword, string sms_Otp)
        {
            var result = FCJsonResult.UnknowFail;
            if (paypassword == confirmpassword)
            {
                try
                {
                    var cmd = new UserResetTradePasswordByTwoFactor(this.CurrentUser.UserID, paypassword, sms_Otp);
                    this.CommandBus.Send(cmd);

                    Session.Remove("resetTradePwdHash");
                    result = FCJsonResult.CreateSuccessResult(this.Lang("Reset your trade password successfully"));
                }
                catch (CommandExecutionException ex)
                {
                    if (ex.ErrorCode == (int)ErrorCode.GAPasswordError)
                        result = FCJsonResult.CreateFailResult(this.Lang("Unable to update your trade password. Your Google Authenticator code error."));
                    else if (ex.ErrorCode == (int)ErrorCode.SMSPasswordError)
                        result = FCJsonResult.CreateFailResult(this.Lang("Unable to update your trade password. Your Sms Authenticator code error."));
                    else
                        Log.Error("Action ResetTradePasswordByTwoFactor Error", ex);
                }
            }

            return Json(result);
        }
        #endregion

    }
}
