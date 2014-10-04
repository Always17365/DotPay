using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using FC.Framework.Utilities;
using DotPay.Domain.Exceptions;
using DotPay.Domain.Events;

namespace DotPay.Domain
{
    public class Membership : DomainBase, IEntity
    {
        #region ctor
        protected Membership()
        {
        }

        public Membership(User owner, string userEmail, string password)
        {
            this.Owner = owner;

            //set the default value   
            this.Email = userEmail;
            this.IsEmailVerify = false;
            this.IsLocked = false;
            this.Password = password;
            this.PasswordResetToken = string.Empty;
            this.TradePasswordResetToken = string.Empty;
            this.TradePassword = string.Empty;
            this.CreateAt = DateTime.Now.ToUnixTimestamp();
            this.IdNo = string.Empty;
            this.IdNoType = default(Common.IdNoType);
            this.RealName = string.Empty;
            this.GenerteEmailValidateToken(userEmail);
        }
        #endregion

        #region properties
        public virtual int UserID { get; protected set; }
        public virtual string Email { get; protected set; }
        public virtual string IdNo { get; protected set; }
        public virtual IdNoType IdNoType { get; protected set; }
        public virtual string RealName { get; protected set; }
        public virtual int RegisterAt { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual bool IsEmailVerify { get; protected set; }
        public virtual string EmailValidateToken { get; protected set; }
        public virtual bool IsLocked { get; protected set; }
        public virtual int LockAt { get; protected set; }
        public virtual int UnLockAt { get; protected set; }

        public virtual string Password { get; set; }
        public virtual int LastPasswordVerifyAt { get; protected set; }
        public virtual int LastReceiveScoreAt { get; protected set; }
        public virtual int LastPasswordFailureAt { get; protected set; }
        public virtual int PasswordChangeAt { get; protected set; }
        public virtual string PasswordResetToken { get; protected set; }

        public virtual string TradePassword { get; set; }
        public virtual int LastTradePasswordVerifyAt { get; protected set; }
        public virtual int LastTradePasswordFailureAt { get; protected set; }
        public virtual int TradePasswordChangeAt { get; protected set; }
        public virtual string TradePasswordResetToken { get; protected set; }

        public virtual User Owner { get; protected set; }
        #endregion

        #region public method
        public virtual bool VerifyLoginPassword(string loginPassword)
        {
            if (this.IsLocked)
                throw new UserIsLockedException();

            var verify = this.Password.Equals(loginPassword);
            var timestamp = DateTime.Now.ToUnixTimestamp();

            if (verify) this.LastPasswordVerifyAt = timestamp;
            else this.LastPasswordFailureAt = timestamp;

            return verify;
        }

        public virtual bool VerifyTradePassword(string tradePassword)
        {
            var timestamp = DateTime.Now.ToUnixTimestamp();
            var verify = this.TradePassword.Equals(tradePassword);

            if (verify) this.LastTradePasswordVerifyAt = timestamp;
            else this.LastTradePasswordFailureAt = timestamp;

            return verify;
        }

        public virtual void VerifyEmail(string token)
        {
            if (!this.IsEmailVerify)
            {
                var verify = this.EmailValidateToken.Equals(token);

                if (verify)
                    this.IsEmailVerify = true;
                else
                    throw new UserIDOfTokenNotMatchException();
            }
        }

        public virtual void ChangePassword(string newPassword)
        {
            this.Password = newPassword;
            this.PasswordChangeAt = DateTime.Now.ToUnixTimestamp();
        }

        public virtual void ChangeTradePassword(string newTradePassword)
        {
            this.TradePassword = newTradePassword;
            this.TradePasswordChangeAt = DateTime.Now.ToUnixTimestamp();
        }

        public virtual void ResetPassword()
        {
            var randomNum = new Random().Next();
            var token = CryptoHelper.MD5("prefix_rp" + this.Email + DateTime.Now.ToUnixTimestamp().ToString() + randomNum);
            var expirationTime = DateTime.Now.AddMinutes(Constants.RESET_PASSWORD_RELATIVE_EXPIRATION_TIME_MINUTES);

            this.PasswordResetToken = token;
            this.RaiseEvent(new TokenGenerated(this.UserID, token, TokenType.PasswordReset, expirationTime));
        }

        public virtual void ResetTradePassword()
        {
            var randomNum = new Random().Next();
            var token = CryptoHelper.MD5("prefix_rtp" + this.Email + DateTime.Now.ToUnixTimestamp().ToString() + randomNum);
            var expirationTime = DateTime.Now.AddMinutes(Constants.RESET_TRADE_PASSWORD_RELATIVE_EXPIRATION_TIME_MINUTES);

            this.TradePasswordResetToken = token;
            this.RaiseEvent(new TokenGenerated(this.UserID, token, TokenType.TradePasswordReset, expirationTime));
        }

        public virtual bool HasRealNameAuthentication()
        {
            return !(string.IsNullOrEmpty(this.RealName) || string.IsNullOrEmpty(this.IdNo) || this.IdNoType == default(DotPay.Common.IdNoType));
        }

        public virtual void RealNameAuthentication(string realName, string idNo, IdNoType idType)
        {
            if (this.HasRealNameAuthentication())
                throw new RealNameAuthenticationIsPassedException();

            this.RealName = realName;
            this.IdNo = idNo;
            this.IdNoType = idType;
        }

        public virtual void Lock()
        {
            if (this.IsLocked) return;

            this.IsLocked = true;
            this.LockAt = DateTime.Now.ToUnixTimestamp();
        }

        public virtual void Unlock()
        {
            if (!this.IsLocked) return;

            this.IsLocked = false;
            this.UnLockAt = DateTime.Now.ToUnixTimestamp();
        }

        public virtual void UpdateLastReceiveScoreDate()
        {
            this.LastReceiveScoreAt = DateTime.Now.ToUnixTimestamp();
        }
        #endregion

        #region private method
        private void GenerteEmailValidateToken(string email)
        {
            this.EmailValidateToken = CryptoHelper.MD5(email + DateTime.Now.ToLongDateString());
        }
        #endregion
    }
}
