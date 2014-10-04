using FC.Framework;
using DotPay.Common;
using DotPay.Command;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.QueryService;
using QConnectSDK.Context;
using QConnectSDK;
using System.IO;

namespace DotPay.Web.Controllers
{
    public class ErrorController : BaseController
    { 
        [Route("~/notfound")]
        [AllowAnonymous]
        public ActionResult Index()
        { 
            return View();
        }
    }
}
