using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Admin.ViewModel;
using Dotpay.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dotpay.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected ICommandBus CommandBus { get { return IoC.Resolve<ICommandBus>(); } }

        protected ManagerIdentity CurrentUser
        {
            get
            {
                return (ManagerIdentity)Session[Constants.CURRENT_USER_KEY];
            }
        }

        protected void SetWaitVerifyTwofactorLoginManager(ManagerIdentity manager)
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

        protected override void EndExecute(IAsyncResult asyncResult)
        {
            ViewBag.CurrentUser = this.CurrentUser;
            base.EndExecute(asyncResult);
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
                    response.Write(JsonConvert.SerializeObject(this.Data, Newtonsoft.Json.Formatting.None, timeFormat)); 
                } 
            }

        }
        #endregion
    }
}