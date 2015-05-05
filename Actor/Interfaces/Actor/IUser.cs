using Orleans;
using System.Threading.Tasks;
using Dotpay.Common;
using System.Collections;
using System.Collections.Generic;
using System;
using Dotpay.Common.Enum;

namespace Dotpay.Actor
{
    public interface IUser : IGrainWithGuidKey
    {
        Task<ErrorCode> Register(string email,string loginPassword, Lang lang, string activeToken);
        Task<ErrorCode> ResetActiveToken(string activeToken);
        Task InitializePaymentPassword(string paymentPassword);
        Task<ErrorCode> Active(string emailToken);
        Task SetLoginName(string loginName);
        Task<Tuple<ErrorCode, int>> Login(string loginPassword, string ip);
        Task Lock(Guid lockBy, string reason);
        Task Unlock(Guid lockBy, string reason);
        Task SetMobile(string mobile, string otpKey, string otp);
        Task SmsCounterIncrease();
        Task VeirfyIdentity(string fullName, IdNoType idNoType, string idNo);
        Task<Tuple<ErrorCode, string>> ForgetLoginPassword(string token);
        Task<ErrorCode> ResetLoginPassword(string newLoginPassword, string resetToken);
        Task<Tuple<ErrorCode, string>> ForgetPaymentPassword(string token);
        Task<ErrorCode> ResetPaymentPassword(string newPaymentPassword, string resetToken);
        Task<ErrorCode> CheckLoginPassword(string loginPassword);
        Task<ErrorCode> CheckPaymentPassword(string paymentPassword);
        Task<ErrorCode> ModifyLoginPassword(string oldLoginPassword, string newLoginPassword);
        Task<ErrorCode> ModifyPaymentPassword(string oldPaymentPassword, string newPaymentPassword, string smsVerifyCode = "");
        Task<Guid> GetAccountId();
        Task<UserInfo> GetUserInfo();
    }

    public class UserInfo
    {
        public UserInfo(string loginName, string email, Lang lang)
        {
            this.LoginName = loginName;
            this.Email = email;
            this.Lang = lang;
        }

        public string LoginName { get; set; }
        public string Email { get; set; }
        public Lang Lang { get; set; }
    }
}
