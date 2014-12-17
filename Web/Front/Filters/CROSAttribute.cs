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
    public class CROSAttribute :System.Web.Http.Filters.ActionFilterAttribute 
    {
        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}