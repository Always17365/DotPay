using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Dotpay.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Welcome()
        {
            return PartialView();
        }
        public ActionResult SidebarNav()
        {
            this.ViewBag.CurrentUser = this.CurrentUser;
            return PartialView();
        }
    }
}