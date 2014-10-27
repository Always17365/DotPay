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
    public class GoogleAuthentication : DomainBase, IEntity
    {
        #region ctor
        protected GoogleAuthentication() { }
        public GoogleAuthentication(int userID, string otpSecret)
        {
            this.UserID = userID;
            this.OTPSecret = otpSecret;
            this.LastVerifyAt = 0;
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
        #endregion

        #region public method

        public virtual bool Verify(string oneTimePassword)
        {
            var timestamp = DateTime.Now.ToUnixTimestamp();
            var verify = Utilities.GenerateGoogleAuthOTP(this.OTPSecret).Equals(oneTimePassword);

            if (verify)
            {
                var now = DateTime.Now.ToUnixTimestamp();
                var nowmod = now % 60;
                var verifymod = this.LastVerifyAt % 60;
                //如果在同一个30秒周期内验证两次，是不允许通过的
                if ((now - this.LastVerifyAt < 30) && ((nowmod <= 30 && verifymod <= 30) || (nowmod > 30 && verifymod > 30)))
                    verify = false;
                else
                    this.LastVerifyAt = timestamp;
            }
            else
            {
                this.LastFailureAt = timestamp;
            }

            return verify;
        }

        public virtual void ChangeSecret(string secret)
        {
            var timestamp = DateTime.Now.ToUnixTimestamp();
            this.OTPSecret = secret;
            this.CreateAt = timestamp;
            this.UpdateAt = timestamp;
        }

        #endregion
    }
}
