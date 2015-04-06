using Orleans;
using System.Threading.Tasks;
using Dotpay.Common;
using System.Collections;
using System.Collections.Generic;
using System;
using Dotpay.Common.Enum;

namespace Dotpay.Actor
{
    public interface IUser : IGrainWithIntegerKey
    {
        Task<ErrorCode> PreRegister(string email);
        Task<Guid> Initialize(string userAccount, string loginPassword, string tradePassword);
        Task<ErrorCode> Login(string loginPassword, string ip);
        Task Lock(long operatorId, string reason);
        Task Unlock(long operatorId, string reason);
        Task SetMobile(string mobile, string otpKey, string otp);
        Task SmsCounterIncrease();
        Task VeirfyIdentity(string fullName, IdNoType idNoType, string idNo);
        Task<string> ForgetLoginPassword();
        Task ResetLoginPassword(string newLoginPassword, string resetToken);
        Task<string> ForgetPaymentPassword();
        Task ResetPaymentPassword(string newPaymentPassword, string resetToken);
        Task<bool> CheckLoginPassword(string loginPassword);
        Task<bool> CheckPaymentPassword(string tradePassword);
        Task<ErrorCode> ChangeLoginPassword(string oldLoginPassword, string newLoginPassword);
        Task<ErrorCode> ChangePaymentPassword(string oldPaymentPassword, string newPaymentPassword, string smsVerifyCode = "");
        Task<Guid> GetAccountId();
        Task<UserInfo> GetUserInfo();
    }

    public class UserInfo
    {
        public UserInfo(string loginName, string email)
        {
            this.LoginName = loginName;
            this.Email = email;
        }

        public string LoginName { get; set; }
        public string Email { get; set; }
    }
}
