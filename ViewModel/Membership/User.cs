using DotPay.Common;
using DotPay.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;

namespace DotPay.ViewModel
{
    public class RegisterModel
    {
        [Required] 
        public string Email { get; set; }

        [Required] 
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)] 
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required] 
        public string Email { get; set; }

        [Required] 
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required]
        //[LocalizedDisplayName(LangKey.ViewModel.User.LoginModel.CAPTCHA)]
        //public string Captcha { get; set; }
    }

    public class ModifyPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required] 
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)] 
        public string ConfirmPassword { get; set; }

        [Required]
        public string GAPassword { get; set; }

        [Required]
        public string SMSPassword { get; set; }
    }

    [Serializable]
    public class LoginUser
    {
        public int UserID { get; set; }
        public string NickName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        //public int ScoreBalance { get; set; }
        public UserVipLevel VipLevel { get; set; }
        public bool IsVerifyEmail { get; set; }
        public bool IsLocked { get; set; }
        public bool IsManager { get { return this.Role > 0; } }
        public bool IsSetTradePassword { get { return !string.IsNullOrEmpty(this.TradePassword); } }
        public string TradePassword { get; set; }
        public int TwoFactorFlg { get; set; }
        public bool IsBindGA { get { return (this.TwoFactorFlg & 1) == 1; } }
        public bool IsOpenLoginGA { get { return (this.TwoFactorFlg & 2) == 2; } }
        public bool IsOpenLoginSMS { get { return (this.TwoFactorFlg & 4) == 4; } }
        public bool IsOpenTwoFactorGA { get { return (this.TwoFactorFlg & 8) == 8; } }
        public bool IsOpenLoginTwoFactor { get { return IsOpenLoginGA || IsOpenLoginSMS; } }
        public bool IsOpenTwoFactorSMS { get { return (this.TwoFactorFlg & 16) == 16; } }
        public bool LoginTwoFactoryVerify { get; set; }
        public int Role { get; set; }
        public string RippleAddress { get; set; }
        public string RippleSecret { get; set; } 
        public string RealName { get; set; }
        public string IdNo { get; set; }
        public IdNoType IdNoType { get; set; }
    }


    public class UserRealNameModel
    {
        public string RealName { get; set; }
        public string IdNo { get; set; }
        public IdNoType IdNoType { get; set; }
    }

    public class SeeUserDepositAmounModel
    {
        public int ID { get; set; }
        public CurrencyType Currency { get; set; }
        public string CurrencyCN { get { return this.Currency.ToString(); } }
        public int CustomerServiceUserID { get; set; }
        public double Amount { get; set; }


    }

    public class UserInListModel
    {
        public int ID { get; set; }
        public string NickName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string VipLevel { get; set; }
        public int ScoreBalance { get; set; }
        public int ScoreUsed { get; set; }
        public int CreateAt { get; set; }
        public int LastPasswordVerifyAt { get; set; }
        public bool IsLocked { get; set; }
    }

    public class ManagerModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string NickName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public ManagerType Type { get; set; }
        public string ManagerType { get { return this.Type.GetDescription(); } }
        public int CreateAt { get; set; }
    }
    public class UserLogInListModel
    {
        public int ID { get; set; }
        public int OperateTime { get; set; }
        public string Memo { get; set; }
    }

    public class UserVipInfoModel
    {
        public UserVipLevel VipLevel { get; set; }
        public int ScoreBalance { get; set; }
        public int ScoreUsed { get; set; }
    }

    public class UsersCurrencyListModel
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public decimal Locked { get; set; }
        public decimal Total { get; set; }
        public int UpdateAt { get; set; }
    }
}
