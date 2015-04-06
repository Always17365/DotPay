using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Dotpay.Common;
using Dotpay.Common.Enum;

namespace Dotpay.Actor.Service
{
    public interface IUserRegisterService : Orleans.IGrainWithIntegerKey
    {
        Task PreRegister(string email, Lang lang);
        Task InitUserInfo(long userId, string userAccount, string loginPassword, string tradePassword);
    }
    #region MessageClass
    public class UserPreRegisterMessage : MqMessage
    {
        public UserPreRegisterMessage(string email, Lang lang)
        {
            this.Email = email;
            this.Lang = lang;
        }

        public string Email { get; set; }
        public Lang Lang { get; set; }
    }

    public class UserInitializedMessage : MqMessage
    {
        public UserInitializedMessage(long userId, Guid accountId)
        {
            this.UserId = userId;
            this.AccountId = accountId;
        }

        public long UserId { get; set; }
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
