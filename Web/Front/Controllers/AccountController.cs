using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;
using DFramework;
using Dotpay.Command;
using Dotpay.Common;
using Dotpay.Front.Validators;
using Dotpay.Front.ViewModel;
using Dotpay.FrontQueryService;

namespace Dotpay.Front.Controllers
{
    public class AccountController : BaseController
    {
        #region 注册
        [Route("~/signup")]
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [Route("~/validate/loginname")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ValidateLoginName(string loginName)
        {
            var query = IoC.Resolve<IUserQuery>();
            var user = await query.GetUserByLoginName(loginName.Trim());
            if (user != null)
                return Json(new { valid = false });
            else
                return Json(new { valid = true });
        }
        [Route("~/validate/email")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ValidateEmail(string email)
        {
            var query = IoC.Resolve<IUserQuery>();
            var user = await query.GetUserByEmail(email.Trim());
            if (user != null)
                return Json(new { valid = false });
            else
                return Json(new { valid = true });
        }
        [Route("~/signup")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(UserRegisterViewModel user)
        {
            var result = DotpayJsonResult.UnknowFail;
            var validator = new UserRegisterViewModelValidator();
            var validResult = validator.Validate(user);

            if (validResult.IsValid)
            {
                var query = IoC.Resolve<IUserQuery>();

                var exist = await query.GetUserByEmail(user.Email.Trim());

                if (exist != null)
                {
                    result = DotpayJsonResult.CreateFailResult(this.Lang("Login name already exists."));
                }
                //else if (users[1] != null)
                //{
                //    result = DotpayJsonResult.CreateFailResult(this.Lang("Email already exists."));
                //}
                else
                {
                    try
                    {
                        var cmd = new UserRegisterCommand(user.Email, user.LoginPassword, GetCurrentLang());
                        await this.CommandBus.SendAsync(cmd);
                        result = DotpayJsonResult.Success;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Register Exception", ex);
                    }
                }
            }
            return Json(result);
        }
        #endregion

        #region 登陆
        [Route("~/signin")]
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [Route("~/signin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignIn(UserLoginViewModel user)
        {
            var result = DotpayJsonResult.UnknowFail;
            var validator = new UserLoginViewModelValidator();
            var validResult = validator.Validate(user);
            var MAX_RETRY_COUNT = 5;

            var retryCounter = (int)(Session[user.LoginName.Trim() + "LOGIN_RETRY_COUNT"] ?? 1);

            if (MAX_RETRY_COUNT - retryCounter <= 0)
                result = DotpayJsonResult.CreateFailResult(
                                   this.Lang("Login fail too much times.You can try again 1 hour later."));
            else if (validResult.IsValid)
            {
                var query = IoC.Resolve<IUserQuery>();
                UserIdentity userIdentity = null;
                if (user.LoginName.IsEmail())
                {
                    userIdentity = await query.GetUserByEmail(user.LoginName.Trim());
                }
                //else if (user.LoginName.IsMobile()) { }//以后用户支持手机号注册
                else
                {
                    userIdentity = await query.GetUserByLoginName(user.LoginName.Trim());
                }

                if (userIdentity != null)
                {
                    try
                    {
                        var cmd = new UserLoginCommand(userIdentity.UserId, user.Password, this.GetUserIpAddress());
                        await this.CommandBus.SendAsync(cmd);

                        if (cmd.CommandResult.Item1 == ErrorCode.None)
                            result = DotpayJsonResult.Success;
                        else if (cmd.CommandResult.Item1 == ErrorCode.UnactiveUser)
                        {
                            this.CurrentUnactiveUserEmail = userIdentity.Email;
                            result = DotpayJsonResult.CreateFailResult(-2, user.LoginName);
                        }
                        else if (cmd.CommandResult.Item1 == ErrorCode.UserAccountIsLocked)
                            result = DotpayJsonResult.CreateFailResult(-3, "");
                        else if (cmd.CommandResult.Item1 == ErrorCode.LoginNameOrPasswordError)
                            result =
                                DotpayJsonResult.CreateFailResult(
                                    this.Lang("Login name or password error. You can try {0} times.",
                                        cmd.CommandResult.Item2.ToString()));
                        else if (cmd.CommandResult.Item1 == ErrorCode.ExceedMaxLoginFailTime)
                            result =
                                DotpayJsonResult.CreateFailResult(
                                    this.Lang("Login fail too much times.You can try again 1 hour later."));

                    }
                    catch (Exception ex)
                    {
                        Log.Error("Register Exception", ex);
                    }
                }
                else
                {
                    Session[user.LoginName.Trim() + "LOGIN_RETRY_COUNT"] = retryCounter + 1;
                    result = DotpayJsonResult.CreateFailResult(this.Lang("Login name or password error. You can try {0} times.", (MAX_RETRY_COUNT - retryCounter).ToString()));
                }
            }
            return Json(result);
        }
        #endregion

        #region 激活
        [Route("~/active")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Active(string user, string token)
        {
            var result = DotpayJsonResult.CreateFailResult("Invlid activation link.");

            if (!user.IsEmpty() && !token.IsEmpty())
            {
                var query = IoC.Resolve<IUserQuery>();
                var userIdentity = await query.GetUserByLoginName(user.Trim());
                if (userIdentity != null)
                {
                    try
                    {
                        var cmd = new UserActiveCommand(userIdentity.UserId, token);
                        await this.CommandBus.SendAsync(cmd);
                        if (cmd.CommandResult == ErrorCode.None)
                            result = DotpayJsonResult.Success;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Active Exception", ex);
                    }
                }
            }
            ViewBag.Result = result;
            return View();
        }
        #endregion

        #region 重发激活邮件
        [Route("~/resend")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResendActiveEmail()
        {
            var result = DotpayJsonResult.CreateFailResult("You can send active email after 15 minutes.");
            var email = this.CurrentUnactiveUserEmail;
            if (!string.IsNullOrEmpty(email))
            {
                var query = IoC.Resolve<IUserQuery>();
                var userIdentity = await query.GetUserByEmail(email);
                if (userIdentity != null)
                {
                    try
                    {
                        var cmd = new UserResendActiveEmailCommand(userIdentity.UserId);
                        await this.CommandBus.SendAsync(cmd);
                        if (cmd.CommandResult == ErrorCode.None)
                            result = DotpayJsonResult.Success;
                        if (cmd.CommandResult == ErrorCode.UserHasActived)
                            result = DotpayJsonResult.CreateFailResult("Account has actived."); ;
                        if (cmd.CommandResult == ErrorCode.UserHasActived)
                            result = DotpayJsonResult.CreateFailResult("You can send active email after 15 minutes.");
                    }
                    catch (Exception ex)
                    {
                        Log.Error("ResendActiveEmail Exception", ex);
                    }
                }
            }
            return Json(result);
        }
        #endregion

    }
}