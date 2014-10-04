
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

        [ChildActionOnly]
        public ActionResult AdminHeader()
        {
            return PartialView("_header", this.CurrentUser);
        }

    }
}
