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
        #region Views
        [Route("~/i")]
        public async Task<ActionResult> Index()
        {
            var accountQuery = IoC.Resolve<IAccountQuery>();
            var txQuery = IoC.Resolve<ITransactionQuery>();
            var currentUser = this.CurrentUser;
            var userBalances = await accountQuery.GetAccountBalanceByOwnerId(currentUser.UserId);
            ViewBag.Balances = userBalances;
            var indexTxs = await txQuery.GetLastTenTransationByAccountId(currentUser.AccountId);
            var dateStart = DateTime.Now.AddDays(-30);
            var end = DateTime.Now;
            var depositSum = await txQuery.SumDepositAmount(currentUser.AccountId, dateStart, end);
            var transferTuple = await txQuery.SumTransferOutAndInAmount(currentUser.AccountId, dateStart, end);



            ViewBag.RecentTxs = indexTxs;
            ViewBag.Out = transferTuple.Item1;
            ViewBag.In = depositSum + transferTuple.Item2;
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


        #region 修改登陆密码 View

        [Route("~/profile/modifyloginpwd")]
        [HttpGet]
        public ActionResult ModifyLoginPassword()
        {
            return View();
        }

        #endregion

        #region 修改支付密码 View

        [Route("~/profile/modifypaypwd")]
        [HttpGet]
        public ActionResult ModifyPaymentPassword()
        {
            return View();
        }

        #endregion
        #endregion

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
                ViewBag.Message = this.Lang("verifyIdentityFirst");
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

        #region 修改登陆密码

        [Route("~/profile/modifyloginpwd")]
        [HttpPost]
        public async Task<ActionResult> ModifyLoginPassword(string oldpassword, string newpassword, string confirmpassword)
        {
            var result = DotpayJsonResult.SystemError;

            if (!string.IsNullOrEmpty(oldpassword) &&
                !string.IsNullOrEmpty(newpassword) &&
                newpassword == confirmpassword)
            {
                try
                {
                    var cmd = new ModifyLoginPasswordCommand(this.CurrentUser.UserId, oldpassword, newpassword);
                    await this.CommandBus.SendAsync(cmd);
                    if (cmd.CommandResult == ErrorCode.None)
                    {
                        result = DotpayJsonResult.Success;
                    }
                    else if (cmd.CommandResult == ErrorCode.OldLoginPasswordError)
                    {
                        result = DotpayJsonResult.CreateFailResult(this.Lang("OldLoginPasswordError"));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("ModifyLoginPassword Exception", ex);
                }
            }

            return Json(result);
        }

        #endregion

        #region 修改支付密码

        [Route("~/profile/modifypaypwd")]
        [HttpPost]
        public async Task<ActionResult> ModifyPaymentPassword(string oldpassword, string newpassword, string confirmpassword)
        {
            var result = DotpayJsonResult.SystemError;

            if (!string.IsNullOrEmpty(oldpassword) &&
                !string.IsNullOrEmpty(newpassword) &&
                newpassword == confirmpassword)
            {
                try
                {
                    var cmd = new ModifyPaymentPasswordCommand(this.CurrentUser.UserId, oldpassword, newpassword);
                    await this.CommandBus.SendAsync(cmd);
                    if (cmd.CommandResult == ErrorCode.None)
                    {
                        result = DotpayJsonResult.Success;
                    }
                    else if (cmd.CommandResult == ErrorCode.OldPaymentPasswordError)
                    {
                        result = DotpayJsonResult.CreateFailResult(this.Lang("OldPaymentPasswordError"));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("ModifyPaymentPassword Exception", ex);
                }
            }

            return Json(result);
        }

        #endregion
    }
}