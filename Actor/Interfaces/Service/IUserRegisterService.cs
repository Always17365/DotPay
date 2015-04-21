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
        Task Register(string email, string loginPassword, Lang lang);
        Task<ErrorCode> ResendActiveEmail(Guid userId);
        Task<ErrorCode> ActiveUser(Guid userId, string token);
    }

    #region MessageClass
    [Immutable]
    [Serializable]
    public class UserRegisterMessage : MqMessage
    {
        public UserRegisterMessage(string email, Lang lang, string activeToken)
        {
            this.Email = email;
            this.Lang = lang;
            this.ActiveToken = activeToken;
        }
        public string Email { get; set; }
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

    public class UserForgetPasswordMessage : MqMessage
    {
        public UserForgetLoginPasswordMessage ToUserForgetLoginPasswordMessage()
        {
            return new UserForgetLoginPasswordMessage
            (this.Email, this.LoginName, this.Token,
               this.Timestamp, this.Lang, this.Type);
        }
        public UserForgetPaymentPasswordMessage ToUserForgetPaymentPasswordMessage()
        {
            return new UserForgetPaymentPasswordMessage
            (this.Email, this.LoginName, this.Token,
               this.Timestamp, this.Lang, this.Type);
        }

        public string Email { get; set; }
        public string LoginName { get; set; }
        public string Token { get; set; }
        public DateTime Timestamp { get; set; }
        public Lang Lang { get; set; }
        public string Type { get; set; }
    }

    [Immutable]
    [Serializable]
    public class UserForgetLoginPasswordMessage : UserForgetPasswordMessage
    {
        public UserForgetLoginPasswordMessage(string email, string loginName, string token, DateTime timestamp, Lang lang, string type = "UserForgetLoginPasswordMessage")
        {
            this.Email = email;
            this.LoginName = loginName;
            this.Token = token;
            this.Timestamp = timestamp;
            this.Lang = lang;
            this.Type = type;
        }
    }
    [Immutable]
    [Serializable]
    public class UserForgetPaymentPasswordMessage : UserForgetPasswordMessage
    {
        public UserForgetPaymentPasswordMessage(string email, string loginName, string token, DateTime timestamp, Lang lang, string type = "UserForgetPaymentPasswordMessage")
        {
            this.Email = email;
            this.LoginName = loginName;
            this.Token = token;
            this.Timestamp = timestamp;
            this.Lang = lang;
            this.Type = type;
        }
    }
    #endregion
}
