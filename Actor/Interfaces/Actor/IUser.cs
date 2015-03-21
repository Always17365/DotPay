using Orleans;
using System.Threading.Tasks;
using Dotpay.Common;
using System.Collections;
using System.Collections.Generic;
using System;
using Dotpay.Common.Enum;

namespace Dotpay.Actor.Interfaces
{
    public interface IUser : IGrainWithGuidKey
    {
        Task<ErrorCode> PreRegister(string email);
        Task Initialize(string userAccount, string loginPassword, string tradePassword);
        Task<ErrorCode> Login(string loginPassword, string ip);
        Task Lock(Guid operatorId, string reason);
        Task Unlock(Guid operatorId, string reason);
        Task SetMobile(string mobile, string otpKey, string otp);
        Task SmsCounterIncrease();
        Task VeirfyIdentity(string fullName, IdNoType idNoType, string idNo); 
        Task ForgetLoginPassword(Lang lang);
        Task ResetLoginPassword(string newLoginPassword, string resetToken);
        Task ForgetPaymentPassword(Lang lang);
        Task ResetPaymentPassword(string newPaymentPassword, string resetToken); 
        Task<bool> CheckLoginPassword(string loginPassword);
        Task<bool> CheckPaymentPassword(string tradePassword);
        Task<ErrorCode> ChangeLoginPassword(string oldLoginPassword, string newLoginPassword);
        Task<ErrorCode> ChangePaymentPassword(string oldPaymentPassword, string newPaymentPassword, string smsVerifyCode = "");
        Task AssignRoles(Guid operatorId, IEnumerable<ManagerType> roles);
    }
}
