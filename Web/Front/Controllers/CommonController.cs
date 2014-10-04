using FC.Framework;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.Web.Controllers;
using DotPay.ViewModel;
using DotPay.QueryService;
using DotPay.Tools.SmsInterface;
using DotPay.Command;

namespace DotPay.Web.Controllers
{
    public class CommnoController : BaseController
    {
        #region 验证码
        [AllowAnonymous]
        [Route("~/cci")]
        public ActionResult CheckCodeImage()
        {
            var code = CheckCode.GenerateCode(4);
            Session["ValidateCode"] = code;

            return File(CheckCode.CreateCheckImage(code), @"image/jpeg");
        }
        #endregion

        #region sendsms
        [Route("~/sendsms")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SendSms(string mobile, SmsUseFor usefor)
        {
            var waitSenonds = Config.Debug ? 0 : 120;
            var waitSenondsUser = Config.Debug ? 0 : 60;
            var isBindMobile = !string.IsNullOrEmpty(mobile);
            var currentUser = this.CurrentUser ?? this.CurrentTmpUser;


            var result = FCJsonResult.CreateFailResult(this.Lang("Unknow Exception,Please refresh the page and try again"));
            if (currentUser != null)
            {
                mobile = string.IsNullOrEmpty(mobile) ? currentUser.Mobile : mobile.Substring(mobile.Length - 11, 11);
                var key =currentUser.UserID+"sendsms";
                if (isBindMobile)
                {
                    var existUserCount = IoC.Resolve<IUserQuery>().CountUserByMobile(mobile);
                    if (existUserCount > 0)
                        return Json(FCJsonResult.CreateFailResult(this.Lang("This mobile {0} is exist.").FormatWith(mobile)));
                }


                object tmp;
                if (Cache.TryGet(key, out tmp))
                {
                    var leaseSeconds = waitSenonds - (DateTime.Now - (DateTime)(tmp)).TotalSeconds;
                    return Json(new { Code = -1, Limit = (int)leaseSeconds });
                }

                try
                {
                    if (!Config.Debug)
                    {
                        Cache.Add(key, DateTime.Now, new TimeSpan(0, 0, waitSenonds));
                    }
                    var otp = string.Empty;
                    if (isBindMobile)
                    {
                        var secretCode = Utilities.GenerateOTPKey();
                        Session["BindSMSSecretCode"] = secretCode;
                        otp = Utilities.GenerateSmsOTP(secretCode, 1);
                    }
                    else
                    {
                        var secretCodePair = IoC.Resolve<IUserQuery>().GetUserSmsSecretByID(currentUser.UserID);
                        otp = Utilities.GenerateSmsOTP(secretCodePair.Item1, secretCodePair.Item2 + 1);
                        //短信计数器 async
                        this.CommandBus.Send(new SmsCounterCommand(currentUser.UserID));
                    }
                    var content = "您的验证码是：{0}。请不要把验证码泄露给其他人。如非本人操作，可不用理会！".FormatWith(otp);

                    if (Config.Debug)
                        Log.Debug("短信验证码-->" + otp);
                    else
                    {
                        var counter = 0;
                        var cacheCouterKey = CacheKey.USER_SMS_DALIY_COUNTER + currentUser.UserID;
                        Cache.TryGet(cacheCouterKey, out counter);

                        if (counter < 5) SmsHelper.Send(mobile, content);
                        else if (counter < 15) SmsHelper.SendByBackup(mobile, content);
                        else result = FCJsonResult.CreateFailResult(this.Lang("You send too much sms,Please try again next day."));

                        counter += 1;
                        var now = DateTime.Now;
                        Cache.Add(cacheCouterKey, counter, new DateTime(now.Year, now.Month, now.Day, 23, 59, 56));
                    }

                    result = FCJsonResult.Success;
                }
                catch (CommandExecutionException ex)
                {
                    Log.Error("Action Send Sms", ex);
                }
            }
            return Json(result);
        }
        #endregion
        public enum SmsUseFor
        {
            SwitchLoginTwofactor = 1,
            BindMobile = 2,
            Withdraw = 3,
            ModifyLoginPassword = 4,
            ModifyTradePwd = 5,
            ResetTradePwd = 6,
            Login2FA = 7,
            ResetLoginPwd=8
        }
    }
}
