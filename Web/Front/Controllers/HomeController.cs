using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dotpay.Front.Controllers
{
    public class HomeController : BaseController
    {
        [Route("~/")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}