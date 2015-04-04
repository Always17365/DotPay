using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Admin.ViewModel;
using Dotpay.Common;

namespace Dotpay.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected ICommandBus CommandBus { get { return IoC.Resolve<ICommandBus>(); } }

        protected ManagerIdentity CurrentUser
        {
            get
            {
                return (ManagerIdentity)Session[Constants.CurrentUserKey] ?? new ManagerIdentity()
                {
                    ManagerId = Guid.NewGuid(),
                    LoginName = "Admin",
                    Roles = new List<ManagerType>() { ManagerType.SuperUser, ManagerType.MaintenanceManager }
                };
            }
        }
    }
}