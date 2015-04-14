using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dotpay.Admin.ViewModel;
using Dotpay.Common;

namespace Dotpay.Admin.Fliter
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
                    filterContext.Result = new RedirectResult("~/account/login");
                } 
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AllowRolesAttribute : AuthorizeAttribute
    {
        public AllowRolesAttribute()
        {
            Order = 2;
        }
        public ManagerType Role { get; set; }
        public ManagerType Role1 { get; set; }
        public ManagerType Role2 { get; set; }
        public ManagerType Role3 { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false))
            {
                var userSession = (ManagerIdentity)(filterContext.HttpContext.Session[Constants.CURRENT_USER_KEY]);

                if (userSession != null && !HasPermission(userSession))
                {
                    filterContext.Result = new PartialViewResult() { ViewName = "~/NoPermission" };
                }
            } 
        }

        private bool HasPermission(ManagerIdentity manager)
        {
            return manager.Roles.Contains(this.Role) ||
                   manager.Roles.Contains(this.Role1) ||
                   manager.Roles.Contains(this.Role2) ||
                   manager.Roles.Contains(this.Role3);
        }
    }
}