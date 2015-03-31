using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dotpay.Common;

namespace Dotpay.Admin.Fliter
{
    public class AuthenticationAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false))
            {
                var userSession = filterContext.HttpContext.Session[Constants.CurrentUserKey];

                //if (userSession == null || !((LoginUser)userSession).LoginTwoFactoryVerify)
                //{
                //    filterContext.Result = new RedirectResult("~/Index");
                //    base.OnAuthorization(filterContext);
                //}
            }
        }
    }
}