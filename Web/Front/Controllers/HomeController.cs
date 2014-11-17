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
using FC.Framework.Utilities;

namespace DotPay.Web.Controllers
{
    public class HomeController : BaseController
    {
        #region const
        const string callback_weibo_url = "http://www.fullcoin.com/weibologin";
        const string callback_qq_url = "http://www.fullcoin.com/qqlogin";
        #endregion

        #region view

        #region index
        [Route("~/")]
        [Route("~/index")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region ripple txt
        [Route("~/ripple.txt")]
        [AllowAnonymous]
        public ActionResult RippleTxt()
        {
            return File("~/App_Data/ripple.txt", "text/plain");
        }
        #endregion

        #region 关于我们
        [Route("~/about")]
        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }
        #endregion

        #region 预注册页
        [Route("~/preregister")]
        [AllowAnonymous]
        public ActionResult PreRegister()
        {
            return View();
        }
        #endregion

        #region 注册页
        [Route("~/register")]
        [AllowAnonymous]
        public ActionResult Register(string email, string token)
        {
            Session["PreRegistrationEmail"] = email;
            Session["PreRegistrationToken"] = token;
            return View();
        }
        #endregion

        #region 登陆页
        [Route("~/login")]
        [Route("~/login/{resetsuccess}")]
        [AllowAnonymous]
        public ActionResult Login(string resetsuccess, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl.NullSafe();
            if (!string.IsNullOrEmpty(resetsuccess))
            {
                ViewBag.ResetSuccess = this.Lang("Your password has been reset successfully. You may login using your new password below");

            }
            return View();
        }
        #endregion

        #region 双重身份验证
        [Route("~/login/twofactor/{verifyhash}")]
        [AllowAnonymous]
        public ActionResult LoginTwoFactor(string verifyhash, string returnUrl)
        {
            ViewData["VerifyHash"] = verifyhash;
            ViewBag.ReturnUrl = returnUrl.NullSafe();
            ViewBag.CurrentTmpUser = this.CurrentTmpUser;
            return View();
        }
        #endregion

        #region 登录日志
        [Route("~/loginHistory")]
        public ActionResult LoginHistory()
        {
            return View("LoginHistory");
        }
        #endregion

        #region Support
        [Route("~/support")]
        public ActionResult Support()
        {
            return View();
        }
        #endregion

        #region ifc
        [Route("~/ifc")]
        [AllowAnonymous]
        public ActionResult IFC()
        {

            return View();
        }
        #endregion

        #region 邮箱激活
        [HttpGet]
        [AllowAnonymous]
        [Route("~/emailActiveSuccess-{code}")]
        public ActionResult ActiveSuccess(int? code)
        {
            if (code.HasValue)
                ViewBag.ErrorMessage = this.Lang("Invalid activation link.Please check the link then try again or contact Support.");
            else
                ViewBag.Message = this.Lang("Account successfully verified. Please login below.");
            return View();
        }
        #endregion

        #region 服务条款
        [Route("~/terms")]
        [AllowAnonymous]
        public ActionResult ServiceRule()
        {
            return View();
        }
        #endregion
        #endregion

        #region Post
        #region ValidateEmail
        [Route("~/ValidateEmail")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ValidateEmail(string email)
        {
            var valid = false;
            email = email.NullSafe().Trim();

            if (!string.IsNullOrEmpty(email) && email.IsEmail())
            {
                valid = !IoC.Resolve<IUserQuery>().ExistUserByEmail(email);
                if (!valid)
                    return Json(new { valid = false, message = "该邮件地址已存在" });
                else
                    return Json(new { valid = true });
            }

            return Json(new { valid = false });
        }
        #endregion

        #region PreRegister
        [Route("~/preregister")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult PreRegister(string email, string checkcode)
        {
            var tmpObj = new object();
            var result = FCJsonResult.UnknowFail;

            if (!CheckImageCode(checkcode, CaptchaType.PreRegistration))
            {
                result = FCJsonResult.CreateFailResult(this.Lang("Verification code error."));
            }
            if (email.NullSafe().Trim().IsEmail())
            {
                if (IoC.Resolve<IUserQuery>().ExistUserByEmail(email))
                    result = FCJsonResult.CreateFailResult(this.Lang("An account with that email address already exists. Please try again or use the forgotten password feature."));
                else
                {
                    try
                    {
                        var cmd = new UserPreRegister(email);
                        this.CommandBus.Send(cmd);
                    }
                    catch (CommandExecutionException ex)
                    {
                        Log.Error("Action PreRegister Error", ex);
                    }
                }
            }

            return Json(result);
        }
        #endregion

        #region Register
        [Route("~/register")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterPost(string password, string tradepassword)
        {
            if (Session["PreRegistrationEmail"] == null || Session["PreRegistrationToken"] == null)
            {
                return Json(FCJsonResult.UnknowFail);
            }
            else
            {
                var email = Session["PreRegistrationEmail"].ToString();
                var token = Session["PreRegistrationToken"].ToString();

                var tmpObj = new object(); 
                var result = FCJsonResult.UnknowFail;


                if (email.NullSafe().IsEmail() && !string.IsNullOrEmpty(password))
                {
                    if (IoC.Resolve<IUserQuery>().ExistUserByEmail(email))
                        result = FCJsonResult.CreateFailResult(this.Lang("An account with that email address already exists. Please try again or use the forgotten password feature."));
                    else
                    {
                        var conmentBy = Session["commendBy"] == null ? 0 : (int)Session["commendBy"];

                        try
                        {
                            var cmd = new UserRegister(email, password, /*address, secret,*/ 8, conmentBy);
                            this.CommandBus.Send(cmd);
                            var loginUser = IoC.Resolve<IUserQuery>().GetUserByEmail(email);
                            Session[Constants.TmpUserKey] = loginUser;

                            this.CurrentUserPassTwoFactoryVerify();
                            result = FCJsonResult.Success;
                        }
                        catch (CommandExecutionException ex)
                        {
                            Log.Error("Action UserRegistter Error", ex);
                        }
                    }
                }

                return Json(result);
            }
        }
        #endregion

        #region login
        [Route("~/login")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, string returnUrl)
        {
            var key = email + this.GetUserIPAddress();
            var retryCount = Cache.Get<int>(key);
            var result = FCJsonResult.CreateFailResult(Language.LangHelpers.Lang("Unknow Exception,Please refresh the page and try again"));
            //记录重试次数，并返回剩余可重试次数的函数 
            Func<string, int, FCJsonResult> RetryCountIncrease = (_key, _retryCount) =>
            {
                Cache.Add(_key, _retryCount + 1, new TimeSpan(2, 0, 0));

                var leaveRetryCount = 5 - _retryCount;
                var errorMsg = Language.LangHelpers.Lang("Invalid username / password. Please try again.You still have {0} chances").FormatWith(leaveRetryCount);
                return FCJsonResult.CreateFailResult(errorMsg);
            };

            if (retryCount > 5)
            {
                var errorMsg = Language.LangHelpers.Lang("Login too frequently,Please try again 2 hours later.");
                result = FCJsonResult.CreateFailResult(errorMsg);
            }
            else
            {
                if (!email.NullSafe().IsEmail() || string.IsNullOrEmpty(password))
                {
                    result = RetryCountIncrease(key, retryCount);
                }
                else
                {
                    try
                    {
                        var cmd = new UserLogin(email, password, this.GetUserIPAddress());
                        this.CommandBus.Send(cmd);

                        //执行成功后，读取用户的信息，保存至Session
                        var loginUser = IoC.Resolve<IUserQuery>().GetUserByEmail(email);

                        if (loginUser.IsManager || loginUser.IsLocked)
                        {
                            result = RetryCountIncrease(key, retryCount);
                        }
                        else
                        {
                            //暂存用户信息
                            var verifyHash = KeepCurrentUserInfoInTmpAndReturnHash(loginUser);
                            //判断用户是否开了双重身份验证
                            var code = 1 | (loginUser.IsOpenLoginGA ? 2 : 0) | (loginUser.IsOpenLoginSMS ? 4 : 0);
                            //移除登录失败统计
                            Cache.Remove(key);

                            if (code > 1)
                            {
                                return Json(new { Code = 2, ReturnUrl = returnUrl.NullSafe(), Hash = verifyHash });
                            }
                            else
                            {
                                this.CurrentUserPassTwoFactoryVerify();
                                return Json(new { Code = 1, ReturnUrl = returnUrl.NullSafe() });
                            }
                        }
                    }
                    catch (CommandExecutionException ex)
                    {
                        if (ex.ErrorCode == (int)ErrorCode.LoginNameOrPasswordError)
                        {
                            result = RetryCountIncrease(key, retryCount);
                        }
                    }
                }
            }
            return Json(result);
        }
        #endregion

        #region Verify two factor
        [Route("~/login/verifytwofactor")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyLoginTwoFactor(string verifyHash, string ga_otp, string sms_otp)
        {
            var result = FCJsonResult.CreateFailResult(Language.LangHelpers.Lang("Unable to verify 2 Factor Authentication. Please try again."));
            var userQuery = IoC.Resolve<IUserQuery>();

            if (this.CheckLoginVerifyHash(verifyHash))
                if (!string.IsNullOrEmpty(ga_otp.NullSafe()) || !string.IsNullOrEmpty(sms_otp.NullSafe()))
                {
                    if (this.CurrentTmpUser.IsOpenLoginGA)
                    {
                        var ga_secret = userQuery.GetUserGoogleAuthenticationSecretByID(this.CurrentTmpUser.UserID);
                        if (!ga_otp.Equals(Utilities.GenerateGoogleAuthOTP(ga_secret)))
                        {
                            result = FCJsonResult.CreateFailResult(this.Lang("Unable to verify 2 Factor Authentication. Your Google Authenticator code error."));
                            return Json(result);
                        }
                    }

                    if (this.CurrentTmpUser.IsOpenLoginSMS)
                    {
                        var sms_secret = userQuery.GetUserSmsSecretByID(this.CurrentTmpUser.UserID);
                        if (!sms_otp.Equals(Utilities.GenerateSmsOTP(sms_secret.Item1, sms_secret.Item2)))
                        {
                            result = FCJsonResult.CreateFailResult(this.Lang("Unable to verify 2 Factor Authentication. Your Sms Authenticator code error."));
                            return Json(result);
                        }
                    }

                    this.CurrentUserPassTwoFactoryVerify();
                    result = FCJsonResult.Success;
                }

            return Json(result);

        }
        #endregion

        #region QQ Open Auth
        [Route("~/goqq")]
        [AllowAnonymous]
        public ActionResult QQLogin()
        {
            this.Session["RETURNURL"] = callback_qq_url;
            var context = new QzoneContext();
            string scope = "get_user_info";
            var authenticationUrl = context.GetAuthorizationUrl("fc", scope);
            return new RedirectResult(authenticationUrl);
        }

        [Route("~/qqlogin")]
        [AllowAnonymous]
        public ActionResult QQCallback()
        {
            if (Request.Params["code"] != null)
            {
                QOpenClient qzone = null;

                var verifier = Request.Params["code"];
                var state = Request.Params["state"];

                if (state == "fc")
                {
                    qzone = new QOpenClient(verifier, state);
                    var qqUser = qzone.GetCurrentUser();
                    var openId = qzone.OAuthToken.OpenId;

                    //var cmd = new UserQQLogin(openId, qqUser.Nickname, this.GetUserIPAddress());
                    //this.CommandBus.Send(cmd);

                    //var loginUser = IoC.Resolve<IUserQuery>().GetUserByOpenID(openId, OpenAuthType.QQ);

                    //if (loginUser.IsLocked) return Redirect("~/index");
                    //else
                    //{
                    //    //暂存用户信息
                    //    var verifyHash = KeepCurrentUserInfoInTmpAndReturnHash(loginUser);
                    //    //判断用户是否开了双重身份验证
                    //    var code = 1 | (loginUser.IsOpenLoginGA ? 2 : 0) | (loginUser.IsOpenLoginSMS ? 4 : 0);
                    //    if (code > 1)
                    //    {
                    //        return Json(new { Code = 2, ReturnUrl = string.Empty, Hash = verifyHash });
                    //    }
                    //    else
                    //    {
                    //        this.CurrentUserPassTwoFactoryVerify();
                    //        return Json(new { Code = 1, ReturnUrl = string.Empty });
                    //    }
                    //}
                }
            }
            return Redirect("~/index");
        }
        #endregion

        #region Weibo Open Auth
        [Route("~/goweibo")]
        [AllowAnonymous]
        public ActionResult WeiboLogin()
        {
            var oauth = new NetDimension.Weibo.OAuth(Config.WeiboAppkey, Config.WeiboAppSecret, callback_weibo_url);
            var authenticationUrl = oauth.GetAuthorizeURL();
            return new RedirectResult(authenticationUrl);
        }

        [Route("~/weibologin")]
        [AllowAnonymous]
        public ActionResult WeiboCallback()
        {
            if (Request.Params["code"] != null)
            {
                var authAode = Request.Params["code"];
                var oauth = new NetDimension.Weibo.OAuth(Config.WeiboAppkey, Config.WeiboAppSecret, callback_weibo_url);
                var weiboClient = new NetDimension.Weibo.Client(oauth);
                var accessToken = oauth.GetAccessTokenByAuthorizationCode(authAode);
                if (!string.IsNullOrEmpty(accessToken.Token))
                {
                    var openId = weiboClient.API.Entity.Account.GetUID();

                    var asd = weiboClient.API.Entity.Users.Show(openId);

                    //var cmd = new UserWeiboLogin(openId, asd.Name, this.GetUserIPAddress());
                    //this.CommandBus.Send(cmd);

                    //var loginUser = IoC.Resolve<IUserQuery>().GetUserByOpenID(openId, OpenAuthType.WEIBO);

                    //if (loginUser.IsLocked) return Redirect("~/index");
                    //else
                    //{
                    //    //暂存用户信息
                    //    var verifyHash = KeepCurrentUserInfoInTmpAndReturnHash(loginUser);
                    //    //判断用户是否开了双重身份验证
                    //    var code = 1 | (loginUser.IsOpenLoginGA ? 2 : 0) | (loginUser.IsOpenLoginSMS ? 4 : 0);
                    //    if (code > 1)
                    //    {
                    //        return Json(new { Code = 2, ReturnUrl = string.Empty, Hash = verifyHash });
                    //    }
                    //    else
                    //    {
                    //        this.CurrentUserPassTwoFactoryVerify();
                    //        return Json(new { Code = 1, ReturnUrl = string.Empty });
                    //    }
                    //}
                }
            }
            return Redirect("~/index");
        }
        #endregion

        #region Logout
        [Route("~/logout")]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            this.Session.Abandon();

            return Redirect("/");
        }
        #endregion

        #region 设置昵称

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetNickName(string nickName)
        {
            if (string.IsNullOrEmpty(nickName.NullSafe()))
                return Json(new FCJsonResult(-1));

            try
            {
                var cmd = new UserSetNickName(this.CurrentUser.UserID, nickName);
                this.CommandBus.Send(cmd);
                this.CurrentUser.NickName = nickName;

                return Redirect("~/myfullcoin");
            }
            catch (CommandExecutionException ex)
            {
                return Json(new FCJsonResult(ex.ErrorCode));
            }
        }
        #endregion

        #region 实名认证
        public ActionResult SetRealNameAuthentication(string truename, IdNoType idNoType, string number)
        {
            if (string.IsNullOrEmpty(truename.NullSafe()) || string.IsNullOrEmpty(number.NullSafe()))
                return Json(new FCJsonResult(-1));

            if (idNoType == IdNoType.IdentificationCard && number.NullSafe().Length < 6 && number.NullSafe().Length > 18)
                return Json(new { Code = -2, Msg = "身份证号不合法" });
            try
            {
                var cmd = new UserRealNameAuth(this.CurrentUser.UserID, truename, idNoType, number);
                this.CommandBus.Send(cmd);
                return Redirect("~/myfullcoin");
            }
            catch (CommandExecutionException ex)
            {
                return Json(new FCJsonResult(ex.ErrorCode));
            }
        }

        #endregion

        #region Email Active
        [Route("~/active/{email}/{token}")]
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ActiveEmail(string email, string token)
        {
            email = HttpUtility.UrlDecode(email.NullSafe());

            if (!email.NullSafe().IsEmail())
            {
                return Redirect("~/emailActiveSuccess-1");
            }

            var user = IoC.Resolve<IUserQuery>().GetUserByEmail(email);

            if (user == null)
            {
                return Redirect("~/emailActiveSuccess-2");
            }
            else
            {
                try
                {
                    var cmd = new UserActiveEmail(user.UserID, token);
                    this.CommandBus.Send(cmd);
                    if (this.CurrentUser != null)
                        this.CurrentUser.IsVerifyEmail = true;

                    return Redirect("~/emailActiveSuccess");
                }
                catch (CommandExecutionException)
                {
                    return Redirect("~/emailActiveSuccess-3");
                }
            }
        }

        #endregion

        #region 获取用户登录日志
        [AllowAnonymous]
        [Route("~/action/LoginHistory")]
        public ActionResult GetUserLoginHistory(int start, int limit)
        {
            var totalCount = IoC.Resolve<ILogsQuery>().CountUserLoginHistory(this.CurrentUser.UserID);
            var result = IoC.Resolve<ILogsQuery>().GetUserLoginHistory(this.CurrentUser.UserID, start, limit);

            return Json(new { data = result, Code = 1, totalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #endregion

        #region 私有方法
        public string GetUserIPAddress()
        {
            var ip = this.Request.Headers["x-forwarded-for"];

            if (string.IsNullOrEmpty(ip))
            {
                Log.Info("通过x-forwarded-for获取用户真实IP失败!");
                ip = this.Request.UserHostAddress;
            }

            return ip;
        }
        #endregion
    }
}
