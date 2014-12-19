
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.Command;
using FC.Framework;

namespace DotPay.Web.Admin.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Admin/Index/
        [Route("~/")]
        [Route("~/Index")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ModalModifyPassword()
        {
            ViewBag.User = this.CurrentUser;
            return PartialView("_modalModifyPassword");
        }

        [HttpPost]
        public ActionResult ModalModifyPassword(string a)
        {
            // var cmd = new UserModifyPassword();

            return Json(1);
        }

        [Route("~/modifyPasswords")]
        [HttpPost]
        public ActionResult ModifyLoginPassword(string oldPassword, string newPassword, string confirmPassword)
        {

            if (oldPassword.Length >= 6 && newPassword.Length >= 6 && confirmPassword == newPassword)
            {
                try
                {
                    var cmd = new UserModifyPassword(this.CurrentUser.UserID, oldPassword, newPassword);
                    this.CommandBus.Send(cmd);
                    return Json(1);
                }
                catch (CommandExecutionException ex)
                {
                    return Json(ex.ErrorCode);
                }
            }
            return Json(0);
        }
        [ChildActionOnly]
        public ActionResult AdminHeader()
        {
            return PartialView("_header", this.CurrentUser);
        }

    }
}
