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
        Task<ErrorCode> Register(string email, string userAccount, string loginPassword, Lang lang, string activeToken);
        Task<ErrorCode> ResetActiveToken(string activeToken);
        Task<ErrorCode> InitializePaymentPassword(string paymentPassword);
        Task<ErrorCode> Active(string emailToken);
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
        Task<bool> CheckLoginPassword(string loginPassword);
        Task<bool> CheckPaymentPassword(string tradePassword);
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
