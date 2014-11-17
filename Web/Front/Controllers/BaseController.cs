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
using System.Threading;

namespace DotPay.Web.Controllers
{
    public class BaseController : Controller
    {
        #region override
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var registerUserCount = IoC.Resolve<IUserQuery>().GetMaxUserID();

            //ViewBag.UserCount = registerUserCount;

            ViewBag.Lang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            ViewBag.CDN = Config.Debug ? string.Empty : Config.CDN;

            if (this.CurrentUser != null)
                this.ViewBag.CurrentUser = this.CurrentUser;

            base.OnActionExecuting(filterContext);
        }

        #endregion

        protected ICommandBus CommandBus { get { return IoC.Resolve<ICommandBus>(); } }

        protected DotPay.Common.Lang CurrentLang
        {
            get
            {
                var result = default(DotPay.Common.Lang);
                var langStr = Thread.CurrentThread.CurrentCulture.Name;
                foreach (var v in Enum.GetValues(typeof(DotPay.Common.Lang)))
                {
                    if (langStr.Replace("-", "").Equals(v.ToString().Replace("_", ""), StringComparison.OrdinalIgnoreCase))
                        result = (DotPay.Common.Lang)v;
                }

                return result;
            }
        }
        public LoginUser CurrentUser
        {
            get
            {
                return Session[Constants.CurrentUserKey] != null ? (LoginUser)Session[Constants.CurrentUserKey] : null;
            }
        }
        /// <summary>
        /// 正处于登录密码验证通过，但未经过双重身份验证的用户信息
        /// </summary>
        public LoginUser CurrentTmpUser
        {
            get
            {
                return Session[Constants.TmpUserKey] != null ? (LoginUser)Session[Constants.TmpUserKey] : null;
            }
        }

        public string Lang(string key, params string[] strs)
        {
            if (string.IsNullOrEmpty(key)) return "?";
            return Language.LangHelpers.Lang(key, strs);
        }

        public string KeepCurrentUserInfoInTmpAndReturnHash(LoginUser user)
        {
            var hash = FC.Framework.Utilities.CryptoHelper.MD5(Guid.NewGuid().Shrink());
            Session[Constants.TmpUserVerifyHash] = hash;
            Session[Constants.TmpUserKey] = user;
            return hash;
        }
        public void CurrentUserPassTwoFactoryVerify()
        {
            Session[Constants.CurrentUserKey] = Session[Constants.TmpUserKey];
            Session.Remove(Constants.TmpUserVerifyHash);
            //Session.Remove(Constants.TmpUserKey);
            this.CurrentUser.LoginTwoFactoryVerify = true;
        }

        public bool CheckLoginVerifyHash(string verifyHash)
        {
            var hash = Session[Constants.TmpUserVerifyHash] == null ? string.Empty : Session[Constants.TmpUserVerifyHash].ToString();

            return hash.Equals(verifyHash.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public bool CheckImageCode(string checkcode, CaptchaType captchaType)
        {
            var key = "CaptchaCode" + captchaType.ToString();
            var code = Session[key] == null ? string.Empty : Session[key].ToString();

            return code.Equals(checkcode.NullSafe().Trim(), StringComparison.OrdinalIgnoreCase);
        }

        protected SelectList GetSelectList<T>(params Enum[] filter)
        {
            var enumValueList = new Dictionary<string, string>();

            foreach (Enum crtEnum in Enum.GetValues(typeof(T)))
            {
                //如果过滤器中包含该枚举值，则跳过此次循环
                if (filter != null && filter.Contains(crtEnum)) continue;

                enumValueList.Add(crtEnum.ToString("D"), crtEnum.ToString());

            }

            return new SelectList(enumValueList, "Key", "Value");
        }

        public enum CaptchaType
        {
            PreRegistration = 1,
            ForgetPassword=2
        }
    }
}
