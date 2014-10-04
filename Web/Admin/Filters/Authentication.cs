using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotPay.Web.Admin.Filters
{
    public class Authentication : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false))
            {
                var userSession = filterContext.HttpContext.Session[Constants.CurrentUserKey];

                if (userSession == null || !((LoginUser)userSession).LoginTwoFactoryVerify)
                {
                    filterContext.Result = new RedirectResult("~/Index");
                    base.OnAuthorization(filterContext);
                }
            }
        }
    }

    public class VerifyIsManagerAttribute : AuthorizeAttribute
    {
        private ManagerType[] roles;
        private VerifyIsManagerAttribute() { }
        public VerifyIsManagerAttribute(params ManagerType[] roles)
        {
            Check.Argument.IsNotEmpty(roles, "roles");

            this.roles = roles;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var userSession = filterContext.HttpContext.Session[Constants.CurrentUserKey];

            if (userSession != null)
            {
                var loginUser = (LoginUser)userSession;
                var verifyPass = false;

                foreach (var role in roles)
                {
                    var roleCode = (int)role;
                    if ((loginUser.Role & roleCode) == roleCode)
                    {
                        verifyPass = true;
                        break;
                    }
                }

                if (!verifyPass)
                {
                    var result = new System.Web.Mvc.JsonResult();
                    result.Data = IoC.Resolve<IJsonSerializer>().Serialize(new DotPay.Web.Admin.JsonResult(-44));
                    filterContext.Result = result;

                    base.OnAuthorization(filterContext);
                }
            }
        }
    }
}