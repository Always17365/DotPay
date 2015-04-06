using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Admin.Fliter;
using Dotpay.AdminCommand;
using Dotpay.AdminQueryService;
using Dotpay.Common;

namespace Dotpay.Admin.Controllers
{

    public class AccountController : BaseController
    {
        public async Task<ActionResult> Login(string loginName, string password)
        {
            var result = DotpayJsonResult.UnknowFail;

            try
            {
                var manangerId = await IoC.Resolve<IManagerQuery>().GetManagerIdByLoginName(loginName);
                var cmd = new ManagerLoginCommand(manangerId, password, this.GetUserIPAddress());
                await this.CommandBus.SendAsync(cmd);
            }
            catch (Exception ex)
            {
                Log.Error("Login Excepiton", ex);
            }

            return Json(result);
        }
    }
}