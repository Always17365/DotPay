using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Common;
using Dotpay.FrontQueryService;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dotpay.Front.Controllers
{
    public class ProfileController : BaseController
    {
        [Route("~/i")]
        public async Task<ActionResult> Index()
        {
            var accountQuery =  IoC.Resolve<IAccountQuery>();
            var userBalances =await accountQuery.GetAccountBalanceByOwnerId(this.CurrentUser.UserId);
            ViewBag.Balances = userBalances;
            return View();
        }
    }
}