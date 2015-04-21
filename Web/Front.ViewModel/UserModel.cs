using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotpay.Front.ViewModel
{

    [Serializable]
    public class UserIdentity
    {
        public Guid UserId { get; set; }
        public string LoginName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
    [Serializable]
    public class UserRegisterViewModel
    {
        public string Email { get; set; }
        public string LoginPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
    [Serializable]
    public class UserLoginViewModel
    {
        public string LoginName { get; set; }//login name 可能是邮箱、用户名或手机号
        public string Password { get; set; }
        public string Captcha { get; set; }
    }
}
