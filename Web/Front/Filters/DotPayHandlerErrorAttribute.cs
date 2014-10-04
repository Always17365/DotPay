using DotPay.Common;
using DotPay.ViewModel;
using DotPay.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotPay.Web.Filters
{
    public class DotPayHandlerErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            //for IIS7
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            var viewdate=new ViewDataDictionary();
            viewdate.Add("ErrorMessage", "处理出错了！请稍候重试");
            if (!filterContext.ExceptionHandled)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = viewdate
                };
                filterContext.ExceptionHandled = true;
            }
        }
    }
}