
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.Command;
using FC.Framework;
using DotPay.Common;
using DotPay.QueryService;

namespace DotPay.Web.Admin.Controllers
{
    public class AuthController : BaseController
    {

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string email, string password)
        {
            //根据返回给浏览器的代码，客户端提示相应的消息
            //只传递代码，可降低服务器和浏览器之间的数据传输量
            //比如-1,客户端可取js多语言文件的某个消息 -->邮箱格式错误
            //    -2,密码不能为空
            //    -3,用户名或密码错误
            //    -4,未知错误,请刷新页面后重试
            //    Success-->代码为1
             
            if (!email.NullSafe().IsEmail()) return Json(new JsonResult(-1));
            if (string.IsNullOrEmpty(password)) return Json(new JsonResult(-2));

            try
            {
                //声明命令
                var cmd = new UserLogin(email, password, this.Request.UserHostAddress);
                //如果命令执行未抛出异常，则意味着命令执行成功
                this.CommandBus.Send(cmd);

                //执行成功后，读取用户的信息，保存至Session
                var loginUser = IoC.Resolve<IUserQuery>().GetUserByEmail(email);
                //loginUser的LoginTwoFactoryVerify属性，表示用户是否通过了第二次的身份验证

                if (loginUser.IsManager)
                {
                    Session[Constants.CurrentUserKey] = loginUser;

                    var code = 1 | (loginUser.IsOpenLoginGA ? 2 : 0) | (loginUser.IsOpenLoginSMS ? 4 : 0);

                    var jsonresult = new JsonResult(code);

                    return Json(jsonresult);
                }
                else return Json(new JsonResult(-3));
            }
            catch (CommandExecutionException ex)
            {
                if (ex.ErrorCode == (int)ErrorCode.LoginNameOrPasswordError)
                {
                    return Json(new JsonResult(-3));
                }
                else
                {
                    return Json(new JsonResult(-4));
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult VerifyTwoFactor(string ga, string sms)
        {
            if (string.IsNullOrEmpty(ga.NullSafe()) && string.IsNullOrEmpty(sms.NullSafe()))
                return Json(new JsonResult(-1));

            var result = 0;
            var userQuery = IoC.Resolve<IUserQuery>();

            if (this.CurrentUser.IsOpenLoginGA)
            {
                var ga_secret = userQuery.GetUserGoogleAuthenticationSecretByID(this.CurrentUser.UserID);
                if (!ga.Equals(Utilities.GenerateGoogleAuthOTP(ga_secret)))
                    result = result | 1;
            }

            if (this.CurrentUser.IsOpenLoginSMS)
            {
                var sms_secret = userQuery.GetUserSmsSecretByID(this.CurrentUser.UserID);
                if (!ga.Equals(Utilities.GenerateSmsOTP(sms_secret.Item1, sms_secret.Item2)))
                    result = result | 2;
            }

            if (result > 0)
                return Json(new JsonResult(result));
            else
            {
                CurrentUserPassTwoFactoryVerify();
                return Json(new JsonResult(8));
            }
        }
    }
}
