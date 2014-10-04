using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotPay.Web.Admin
{
    public class AntiForgeryValidate : ActionFilterAttribute
    {


        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            string cookieToken = "";
            string formToken = "";

            var tokenHeaders = actionContext.HttpContext.Request.Headers.GetValues("RequestVerificationToken");
            if (tokenHeaders != null && tokenHeaders.Count() > 0)
            {
                string[] tokens = tokenHeaders.First().Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }
            System.Web.Helpers.AntiForgery.Validate(cookieToken, formToken);

            base.OnActionExecuting(actionContext);
        }
    }
}