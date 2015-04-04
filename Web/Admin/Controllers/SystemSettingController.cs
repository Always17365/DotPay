using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Admin.Validators;
using Dotpay.Admin.ViewModel;
using Dotpay.AdminCommand;
using Dotpay.AdminQueryService;

namespace Dotpay.Admin.Controllers
{
    public class SystemSettingController : BaseController
    {
        [Route("~/ajax/systemsetting/tofi")]
        [HttpPost]
        public async Task<ActionResult> ToFI()
        {
            var tofiSetting = await IoC.Resolve<ISystemSettingQuery>().GetToFISetting();
            return PartialView(tofiSetting);
        }

        [Route("~/ajax/systemsetting/todotpay")]
        [HttpPost]
        public async Task<ActionResult> ToDotpay()
        {
            var toDotpaySetting = await IoC.Resolve<ISystemSettingQuery>().GetToDotpaySetting();
            return PartialView(toDotpaySetting);
        }

        [Route("~/ajax/systemsetting/tofi/save")]
        [HttpPost]
        public async Task<ActionResult> SaveToFISetting(ToFISettingViewModel fiSetting)
        {
            var validator = new ReceiveRippleTransferSettingViewModelValidator();
            var validateResult = validator.Validate(fiSetting);
            var actionResult = DotpayJsonResult.UnknowFail;
            if (validateResult.IsValid)
            {
                try
                {
                    var cmd = new UpdateToFISettingCommand(fiSetting.MinAmount, fiSetting.MaxAmount, fiSetting.FixedFee,
                        fiSetting.FeeRate, fiSetting.MinFee, fiSetting.MaxFee, this.CurrentUser.ManagerId);

                    await this.CommandBus.SendAsync(cmd);
                    actionResult = DotpayJsonResult.Success;
                }
                catch (Exception ex)
                {
                    Log.Error("SaveToFISetting Excepiton", ex);
                }
            }
            else
            {
                var error = validateResult.Errors.First();
                actionResult = DotpayJsonResult.CreateFailResult(error.ErrorMessage);
            }

            return Json(actionResult);
        }

        [Route("~/ajax/systemsetting/todotpay/save")]
        [HttpPost]
        public async Task<ActionResult> SaveToDotpaySetting(ToDotpaySettingViewModel dotpaySetting)
        {
            var validator = new ReceiveRippleTransferSettingViewModelValidator();
            var validateResult = validator.Validate(dotpaySetting);
            var actionResult = DotpayJsonResult.UnknowFail;
            if (validateResult.IsValid)
            {
                try
                {
                    var cmd = new UpdateToDotpaySettingCommand(dotpaySetting.MinAmount, dotpaySetting.MaxAmount,
                        dotpaySetting.FixedFee, dotpaySetting.FeeRate, dotpaySetting.MinFee, dotpaySetting.MaxFee,
                        this.CurrentUser.ManagerId);

                    await this.CommandBus.SendAsync(cmd);
                    actionResult = DotpayJsonResult.Success;
                }
                catch (Exception ex)
                {
                    Log.Error("SaveToFISetting Excepiton", ex);
                }
            }
            else
            {
                var error = validateResult.Errors.First();
                actionResult = DotpayJsonResult.CreateFailResult(error.ErrorMessage);
            }

            return Json(actionResult);
        }
    }
}