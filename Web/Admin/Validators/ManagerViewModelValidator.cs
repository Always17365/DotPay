using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dotpay.Admin.ViewModel;
using FluentValidation;

namespace Dotpay.Admin.Validators
{
    public class ManagerViewModelValidator : AbstractValidator<ManagerLoginViewModel>
    {
        public ManagerViewModelValidator()
        {
            RuleFor(x => x.LoginName).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
        }
    }
    public class CreateManagerViewModelValidator : AbstractValidator<CreateManagerViewModel>
    {
        public CreateManagerViewModelValidator()
        {
            RuleFor(x => x.LoginName).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.LoginPassword).NotEmpty().WithMessage("密码不能为空");
        }
    }
    public class AssignRoleViewModelValidator : AbstractValidator<AssignRoleViewModel>
    {
        public AssignRoleViewModelValidator()
        {
            RuleFor(x => x.ManagerId).NotEmpty().WithMessage("管理员Id为空");
            RuleFor(x => x.Roles).NotEmpty().WithMessage("分配的角色不能为空");
        }
    }
}