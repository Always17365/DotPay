using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans.Concurrency;

namespace Dotpay.Actor.Service
{
    public interface IUserRegisterService : Orleans.IGrainWithIntegerKey
    {
        Task Register(string loginName, string email, string loginPassword, Lang lang); 
        Task<ErrorCode> ResendActiveEmail(Guid userId);
        Task<ErrorCode> ActiveUser(Guid userId,string token);
    }

    #region MessageClass
    [Immutable]
    [Serializable]
    public class UserRegisterMessage : MqMessage
    {
        public UserRegisterMessage(string email, string loginName, Lang lang, string activeToken)
        {
            this.Email = email;
            this.LoginName = loginName;
            this.Lang = lang;
            this.ActiveToken = activeToken;
        }
        public string Email { get; set; }
        public string LoginName { get; set; }
        public Lang Lang { get; set; }
        public string ActiveToken { get; set; }
    }

    [Immutable]
    [Serializable]
    public class UserActivedMessage : MqMessage
    {
        public UserActivedMessage(Guid userId, Guid accountId)
        {
            this.UserId = userId;
            this.AccountId = accountId;
        }

        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
    }

    public abstract class UserResetPasswordMessage : MqMessage
    {
        public string Email { get; set; }
        public string LoginName { get; set; }
        public string Token { get; set; }
        public DateTime Timestamp { get; set; }
        public Lang Lang { get; set; }
    }

    [Immutable]
    [Serializable]
    public class UserForgetLoginPasswordMessage : UserResetPasswordMessage
    {
        public UserForgetLoginPasswordMessage(string email, string loginName, string token, DateTime timestamp, Lang lang)
        {
            this.Email = email;
            this.LoginName = loginName;
            this.Token = token;
            this.Timestamp = timestamp;
            this.Lang = lang;
        }
    }
    [Immutable]
    [Serializable]
    public class UserForgetPaymentPasswordMessage : UserResetPasswordMessage
    {
        public UserForgetPaymentPasswordMessage(string email, string loginName, string token, DateTime timestamp, Lang lang)
        {
            this.Email = email;
            this.LoginName = loginName;
            this.Token = token;
            this.Timestamp = timestamp;
            this.Lang = lang;
        }
    }
    #endregion
}
