using System;
using System.Collections.Generic;
using DFramework.Utilities;
using Dotpay.Common;
using Dotpay.Common.Enum;

namespace Dotpay.Command
{
    #region User Register Command
    public class UserRegisterCommand : DFramework.Command
    {
        public UserRegisterCommand(string email, string loginPassword, Lang lang)
        {
            Check.Argument.IsNotEmpty(email, "email");
            Check.Argument.IsNotEmpty(loginPassword, "loginPassword");
            Check.Argument.IsNotNegativeOrZero((int)lang, "lang");

            this.Email = email;
            this.LoginPassword = loginPassword;
            this.Lang = lang;
        }

        public string LoginPassword { get; private set; }
        public string Email { get; private set; }
        public Lang Lang { get; private set; }
    }
    #endregion

    #region User Active Command
    public class UserActiveCommand : Command<ErrorCode>
    {
        public UserActiveCommand(Guid userId, string token)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotEmpty(token, "token");

            this.UserId = userId;
            this.Token = token;
        }
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
    }
    #endregion

    #region User Resned Active Email Command
    public class UserResendActiveEmailCommand : Command<ErrorCode>
    {
        public UserResendActiveEmailCommand(Guid userId)
        {
            Check.Argument.IsNotEmpty(userId, "userId");

            this.UserId = userId;
        }
        public Guid UserId { get; private set; }
    }
    #endregion

    #region User Login Command
    public class UserLoginCommand : Command<Tuple<ErrorCode, int>>
    {
        public UserLoginCommand(Guid userId, string loginPassword, string ip)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotEmpty(loginPassword, "loginPassword");
            Check.Argument.IsNotEmpty(ip, "ip");

            this.UserId = userId;
            this.LoginPassword = loginPassword;
            this.Ip = ip;
        }


        public Guid UserId { get; private set; }
        public string LoginPassword { get; private set; }
        public string Ip { get; private set; }
    }
    #endregion

    #region Initalize Payment Password Command
    public class InitalizePaymentPasswordCommand : DFramework.Command
    {
        public InitalizePaymentPasswordCommand(Guid userId, string paymentPassword)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotEmpty(paymentPassword, "paymentPassword");
            this.UserId = userId;
            this.PaymentPassword = paymentPassword;
        }

        public Guid UserId { get; private set; }
        public string PaymentPassword { get; private set; }
    }
    #endregion

    #region Forget Login Password Command
    public class ForgetLoginPasswordCommand : Command<ErrorCode>
    {
        public ForgetLoginPasswordCommand(Guid userId, Lang lang)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotNegativeOrZero((int)lang, "lang");

            this.UserId = userId;
            this.Lang = lang;
        }

        public Guid UserId { get; private set; }
        public Lang Lang { get; private set; }
    }

    #endregion

    #region Reset Login Password Command
    public class ResetLoginPasswordCommand : Command<ErrorCode>
    {
        public ResetLoginPasswordCommand(Guid userId, string newLoginPassword, string token)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotEmpty(newLoginPassword, "newLoginPassword");
            Check.Argument.IsNotEmpty(token, "token");

            this.UserId = userId;
            this.NewLoginPassword = newLoginPassword;
            this.Token = token;
        }

        public Guid UserId { get; private set; }
        public string NewLoginPassword { get; private set; }
        public string Token { get; private set; }
    }

    #endregion

    #region Modify Login Password Command
    public class ModifyLoginPasswordCommand : Command<ErrorCode>
    {
        public ModifyLoginPasswordCommand(Guid userId, string oldLoginPassword, string newLoginPassword)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotEmpty(oldLoginPassword, "oldLoginPassword");
            Check.Argument.IsNotEmpty(newLoginPassword, "newLoginPassword");

            this.UserId = userId;
            this.OldLoginPassword = oldLoginPassword;
            this.NewLoginPassword = newLoginPassword;
        }

        public Guid UserId { get; private set; }
        public string OldLoginPassword { get; private set; }
        public string NewLoginPassword { get; private set; }
    }
    #endregion

    #region Forget Payment Password Command
    public class ForgetPaymentPasswordCommand : Command<ErrorCode>
    {
        public ForgetPaymentPasswordCommand(Guid userId, Lang lang)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotNegativeOrZero((int)lang, "lang");

            this.UserId = userId;
            this.Lang = lang;
        }

        public Guid UserId { get; private set; }
        public Lang Lang { get; private set; }
    }

    #endregion

    #region Reset Payment Password Command
    public class ResetPaymentPasswordCommand : Command<ErrorCode>
    {
        public ResetPaymentPasswordCommand(Guid userId, string newPaymentPassword, string token)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotEmpty(newPaymentPassword, "newPaymentPassword");
            Check.Argument.IsNotEmpty(token, "token");

            this.UserId = userId;
            this.NewPaymentPassword = newPaymentPassword;
            this.Token = token;
        }

        public Guid UserId { get; private set; }
        public string NewPaymentPassword { get; private set; }
        public string Token { get; private set; }
    }

    #endregion

    #region Modify Payment Password Command
    public class ModifyPaymentPasswordCommand : Command<ErrorCode>
    {
        public ModifyPaymentPasswordCommand(Guid userId, string oldPaymentPassword, string newPaymentPassword)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotEmpty(oldPaymentPassword, "oldPaymentPassword");
            Check.Argument.IsNotEmpty(newPaymentPassword, "newPaymentPassword");

            this.UserId = userId;
            this.OldPaymentPassword = oldPaymentPassword;
            this.NewPaymentPassword = newPaymentPassword;
        }

        public Guid UserId { get; private set; }
        public string OldPaymentPassword { get; private set; }
        public string NewPaymentPassword { get; private set; }
    }
    #endregion

    #region User Identity Verify Command
    public class UserIdentityVerifyCommand : DFramework.Command
    {
        public UserIdentityVerifyCommand(Guid userId,string fullname, string idno, IdNoType idType)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotEmpty(fullname, "fullname");
            Check.Argument.IsNotEmpty(idno, "idno");
            Check.Argument.IsNotNegativeOrZero((int)idType, "idType");

            this.UserId = userId;
            this.FullName = fullname;
            this.IdNo = idno;
            this.IdNoType = idType;
        } 
        public Guid UserId { get; private set; }
        public string FullName { get; private set; }
        public string IdNo { get; private set; }
        public IdNoType IdNoType { get; private set; }
    }
    #endregion
}
