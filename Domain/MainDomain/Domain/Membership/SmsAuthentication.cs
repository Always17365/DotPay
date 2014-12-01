using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using FC.Framework.Utilities;
using DotPay.MainDomain.Events;

namespace DotPay.MainDomain
{
    public class SmsAuthentication : DomainBase, IEntity
    {
        #region ctor
        protected SmsAuthentication() { }
        public SmsAuthentication(int userID, string otpSecret)
        {
            this.UserID = userID;
            this.OTPSecret = otpSecret;
            this.LastVerifyAt = 0;
            this.SmsCounter = 1;
            this.CreateAt = DateTime.Now.ToUnixTimestamp();
        }
        #endregion

        #region properties
        public virtual int UserID { get; protected set; }
        public virtual string OTPSecret { get; protected set; }
        public virtual int LastVerifyAt { get; protected set; }
        public virtual int LastFailureAt { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int UpdateAt { get; protected set; }
        public virtual int SmsCounter { get; protected set; }
        #endregion

        #region public method
        public virtual bool Verify(string oneTimePassword)
        {
            var timestamp = DateTime.Now.ToUnixTimestamp();
            var verify = Utilities.GenerateSmsOTP(this.OTPSecret, this.SmsCounter).Equals(oneTimePassword);

            if (verify)
            {
                this.SmsCounter += 1;
                this.LastVerifyAt = timestamp;
            }
            else
            {
                this.LastFailureAt = timestamp;
            }

            return verify;
        }

        public virtual void CounterAdd()
        {

            this.SmsCounter += 1;
        }

        public virtual void SetOTPSecret(string otpSecret)
        {
            this.OTPSecret = otpSecret;
            this.SmsCounter = 1;
        }
        #endregion
    }
}
