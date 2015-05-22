using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Dotpay.Front.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dotpay.Front.Controllers
{
    public class BaseController : Controller
    {
        protected ICommandBus CommandBus { get { return IoC.Resolve<ICommandBus>(); } }

        protected UserIdentity CurrentUser
        {
            get
            {
                return (UserIdentity)Session[Constants.CURRENT_USER_KEY + ""];
            }
        }

        protected void SetCurrentUser(UserIdentity manager)
        {
            Session[Constants.CURRENT_USER_WAIT_VERIFY_TWO_FACTOR_KEY] = manager;
        }

        protected void PassVerifyTwofactor()
        {
            Session[Constants.CURRENT_USER_KEY] = Session[Constants.CURRENT_USER_WAIT_VERIFY_TWO_FACTOR_KEY];
            Session[Constants.CURRENT_USER_WAIT_VERIFY_TWO_FACTOR_KEY] = null;
        }

        protected string GetUserIpAddress()
        {
            var ip = this.Request.Headers["x-forwarded-for"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = this.Request.UserHostAddress;
            }

            return ip;
        }
        protected string CurrentUnactiveUserEmail
        {
            get
            {
                return Session["UNACTIVE_USERID"] != null ? Session["UNACTIVE_USERID"].ToString() : null;
            }
            set
            {
                Session["UNACTIVE_USERID"] = value;
            }
        }


        protected Lang GetCurrentLang()
        {
            var cultureName = Thread.CurrentThread.CurrentCulture.Name;
            switch (cultureName)
            {
                case "zh-CN":
                    return Common.Enum.Lang.SimplifiedChinese;
                case "en-US":
                    return Common.Enum.Lang.English;
                case "zh-TW":
                    return Common.Enum.Lang.TraditionalChinese;
                default:
                    return Common.Enum.Lang.Unkown;
            }
        }

        protected string GetLangName(Lang lang)
        { 
            switch (lang)
            {
                case Common.Enum.Lang.SimplifiedChinese :
                    return "zh-CN";
                case Common.Enum.Lang.English:
                    return  "en-US";
                case Common.Enum.Lang.TraditionalChinese:
                    return  "zh-TW";
                default:
                    return "";
            }
        }
        protected string Lang(string key, params string[] strs)
        {
            if (string.IsNullOrEmpty(key)) return "?";
            return Language.LangHelpers.Lang(key, strs);
        }

        protected override void EndExecute(IAsyncResult asyncResult)
        {
            var cultureName = Thread.CurrentThread.CurrentCulture.Name.ToLower();
            ViewBag.CurrentUser = this.CurrentUser;
            ViewBag.Lang = cultureName;
            ViewBag.Debug = DotpayConfig.Debug;
            base.EndExecute(asyncResult);
        }

        protected void SetCurrentLanaguage(string lang)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }

        #region Json

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return new CustomResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new CustomResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        [Serializable]
        public class CustomResult : JsonResult
        {
            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                HttpResponseBase response = context.HttpContext.Response;
                if (!string.IsNullOrEmpty(this.ContentType))
                {
                    response.ContentType = this.ContentType;
                }
                else
                {
                    response.ContentType = "application/json";
                }
                if (this.ContentEncoding != null)
                {
                    response.ContentEncoding = this.ContentEncoding;
                }
                if (this.Data != null)
                {
                    IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                    timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    response.Write(JsonConvert.SerializeObject(this.Data, Formatting.None, timeFormat));
                }
            }

        }
        #endregion
    }

    public static class HtmlHelperExtension
    {
        public static SelectList SelectListFor<T>(this HtmlHelper html, Enum[] filter)
        {
            var enumValueList = new Dictionary<string, string>();

            foreach (Enum crtEnum in Enum.GetValues(typeof(T)))
            {
                //如果过滤器中包含该枚举值，则跳过此次循环
                if (filter != null && filter.Contains(crtEnum)) continue;

                enumValueList.Add(crtEnum.ToString("D"),  Language.LangHelpers.Lang(crtEnum.ToLangString()));

            }

            return new SelectList(enumValueList, "Key", "Value");
        }

        public static Lang GetLang(this HtmlHelper html )
        {
            var cultureName = Thread.CurrentThread.CurrentCulture.Name;
            switch (cultureName)
            {
                case "zh-CN":
                    return Dotpay.Common.Enum.Lang.SimplifiedChinese;
                case "en-US":
                    return Dotpay.Common.Enum.Lang.English;
                case "zh-TW":
                    return Dotpay.Common.Enum.Lang.TraditionalChinese;
                default:
                    return Dotpay.Common.Enum.Lang.Unkown;
            }
        }
    }

}