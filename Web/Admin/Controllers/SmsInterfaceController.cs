using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using DotPay.QueryService;
using FC.Framework;
using DotPay.ViewModel;
using DotPay.Command;
using DotPay.Common;
using DotPay.Web.Admin.Filters;

namespace DotPay.Web.Admin.Controllers
{
    public class SmsInterfaceController : BaseController
    {

        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager)]
        public ActionResult Index()
        {
            ViewData["IsSuperManager"] = IsSuperManager;
            return View();
        }

        public ActionResult Manager()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetSmsInterface()
        {
            var smsInterface = IoC.Resolve<ISmsInterfaceQuery>().GetSmsInterface();

            return Json(smsInterface);
        }

        [HttpPost]
        public ActionResult CreateSmsInterface(string account, string password)
        {
            if (string.IsNullOrEmpty(account)) return Json(new JsonResult(-1));
            if (string.IsNullOrEmpty(password)) return Json(new JsonResult(-2));

            bool exist = IoC.Resolve<ISmsInterfaceQuery>().ExistSmsInterface();

            if (exist) return Json(new JsonResult(-3));
            else
            {
                try
                {
                    var cmd = new CreateSmsInterface(SmsInterfaceType.IHUYI, account, password, this.CurrentUser.UserID);
                    this.CommandBus.Send(cmd);
                    return Json(JsonResult.Success);
                }
                catch (CommandExecutionException ex)
                {
                    if (ex.ErrorCode == (int)ErrorCode.NoPermission)
                        return Json(new JsonResult(-3));
                    else return Json(new JsonResult(ex.ErrorCode));
                }
            }
        }

        [HttpPost]
        public ActionResult ModifySmsInterface(int smsInterfaceID, string account, string password)
        {
            if (string.IsNullOrEmpty(account)) return Json(new JsonResult(-1));
            if (string.IsNullOrEmpty(password)) return Json(new JsonResult(-2));

            try
            {
                var cmd = new ModifySmsInterface(smsInterfaceID, SmsInterfaceType.IHUYI, account, password, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);
                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                if (ex.ErrorCode == (int)ErrorCode.NoPermission)
                    return Json(new JsonResult(-3));
                else return Json(new JsonResult(ex.ErrorCode));
            }
        }
    }
}
