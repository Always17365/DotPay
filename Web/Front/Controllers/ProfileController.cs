using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Command;
using Dotpay.Common;
using Dotpay.Front.Validators;
using Dotpay.Front.ViewModel;
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
            var accountQuery = IoC.Resolve<IAccountQuery>();
            var txQuery = IoC.Resolve<ITransactionQuery>();
            var userBalances = await accountQuery.GetAccountBalanceByOwnerId(this.CurrentUser.UserId);
            ViewBag.Balances = userBalances;
            var indexTxs = await txQuery.GetLastTenTransationByAccountId(this.CurrentUser.AccountId);

            ViewBag.RecentTxs = indexTxs;
            return View();
        }

        public ActionResult ProfileHeader(int activeIndex)
        {
            ViewBag.ActiveIndex = activeIndex;
            return View();
        }

        [Route("~/profile")]
        [Route("~/profile/index")]
        public async Task<ActionResult> ProfileIndex()
        {
            var accountQuery = IoC.Resolve<IAccountQuery>();
            var userBalances = await accountQuery.GetAccountBalanceByOwnerId(this.CurrentUser.UserId);
            ViewBag.Balances = userBalances;
            return View();
        }

        #region 设置支付密码
        [Route("~/profile/setpaymentpassword")]
        [HttpGet] 
        public ActionResult SetPaymentPassword(string source)
        {
            ViewBag.Message = this.Lang("transferPaymentPasswordNotInit");
            return View();
        }

        #endregion

        #region 实名认证
        [HttpGet]
        [Route("~/profile/identityverify")]
        public ActionResult IdentityVerify(string source)
        {
            if (source.NullSafe().Trim().Equals("transfer"))
                ViewBag.Message =this.Lang("verifyIdentityFirst");
            return View();
        }

        [HttpPost]
        [Route("~/profile/identityverify")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IdentityVerify(IdentityInfoViewModel identity)
        {
            var validator = new IdentityInfoViewModelValidator();
            var validResult = validator.Validate(identity);
            var result = DotpayJsonResult.SystemError;
            if (validResult.IsValid)
            {
                var cmd = new UserIdentityVerifyCommand(this.CurrentUser.UserId, identity.FullName, identity.IdNo, identity.IdType);

                await this.CommandBus.SendAsync(cmd);
                this.CurrentUser.IdentityInfo = identity;
                result = DotpayJsonResult.Success;
            }

            return Json(result);
        }

        #endregion
    }
}