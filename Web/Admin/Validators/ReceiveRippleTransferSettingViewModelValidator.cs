using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dotpay.Admin.ViewModel;
using FluentValidation;

namespace Dotpay.Admin.Validators
{
    public class ReceiveRippleTransferSettingViewModelValidator : AbstractValidator<ReceiveRippleTransferSettingViewModel>
    {
        public ReceiveRippleTransferSettingViewModelValidator()
        {
            RuleFor(x => x.MinAmount).GreaterThanOrEqualTo(0).WithMessage("金额不能小于0");
            RuleFor(x => x.MaxAmount).GreaterThanOrEqualTo(100).WithMessage("金额不能小于100");
            RuleFor(x => x.FixedFee).GreaterThanOrEqualTo(0).WithMessage("固定费用不能小于0");
            RuleFor(x => x.FeeRate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100).WithMessage("费率应在0%-100%之间");
            RuleFor(x => x.MinFee).GreaterThanOrEqualTo(0).WithMessage("最低转账费用不能低于0");
            RuleFor(x => x.MaxFee).GreaterThanOrEqualTo(10).WithMessage("最高转账费用不能小于10");
        }
    }
}