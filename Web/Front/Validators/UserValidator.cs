using Dotpay.Common;
using Dotpay.Front.ViewModel;
using FluentValidation;

namespace Dotpay.Front.Validators
{
    #region UserRegisterViewModelValidator
    public class UserRegisterViewModelValidator : AbstractValidator<UserRegisterViewModel>
    {
        public UserRegisterViewModelValidator()
        {
            //validator中的消息不用返回，只是在客户端验证失效的情况下，配合记录日志使用
            //RuleFor(x => x.LoginName).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email不能为空");
            RuleFor(x => x.LoginPassword).NotEmpty().WithMessage("密码不能为空");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.LoginPassword).WithMessage("两次密码输入不一致");
        }
    }
    #endregion

    #region UserLoginViewModelValidator
    public class UserLoginViewModelValidator : AbstractValidator<UserLoginViewModel>
    {
        public UserLoginViewModelValidator()
        {
            //validator中的消息不用返回，只是在客户端验证失效的情况下，配合记录日志使用
            RuleFor(x => x.LoginName).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
        }
    }
    #endregion

    #region UserLoginViewModelValidator
    public class IdentityInfoViewModelValidator : AbstractValidator<IdentityInfoViewModel>
    {
        public IdentityInfoViewModelValidator()
        {
            //validator中的消息不用返回，只是在客户端验证失效的情况下，配合记录日志使用
            RuleFor(x => x.FullName).NotEmpty().WithMessage("姓名不能为空");
            RuleFor(x => x.IdNo).NotEmpty().WithMessage("证件号码不能为空");
            RuleFor(x => x.IdType).NotEqual(x => default(IdNoType)).WithMessage("证件类型不能为空");
        }
    }
    #endregion
}
