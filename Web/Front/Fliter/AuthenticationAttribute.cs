using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dotpay.Common;

namespace Dotpay.Front.Fliter
{
    public class AuthenticationAttribute : AuthorizeAttribute
    {
        public AuthenticationAttribute()
        {
            Order = 1;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false))
            {
                var userSession = filterContext.HttpContext.Session[Constants.CURRENT_USER_KEY];

#if DEBUG
                if (userSession == null)
#else
                if (userSession == null || !((ManagerIdentity)userSession).LoginTwoFactoryVerify)
#endif
                {
                    filterContext.Result = new RedirectResult("~/signin");
                }
            }
        }
    }

}