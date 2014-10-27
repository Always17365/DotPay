using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Events;
namespace DotPay.MainDomain
{

    public class Token : DomainBase, IAggregateRoot,
                      IEventHandler<TokenUsed>
    {
        #region ctor
        protected Token() { }

        public Token(int userID, string tokenValue, TokenType tokenType, DateTime expirationTime)
        {
            this.UserID = userID;
            this.Value = tokenValue;
            this.Type = tokenType;
            this.ExpiredAt = expirationTime.ToUnixTimestamp();
            this.IsUsed = false;
            this.CreateAt = DateTime.Now.ToUnixTimestamp();
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual string Value { get; protected set; }
        public virtual TokenType Type { get; protected set; }
        public virtual bool IsUsed { get; protected set; }
        public virtual int ExpiredAt { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int UpdateAt { get; protected set; }
        #endregion

        #region public method
        public virtual void MarkUsed()
        {
            if (this.IsUsed || this.ExpiredAt < DateTime.Now.ToUnixTimestamp())
                throw new TokenIsUsedOrTimeOutException();
            else
                this.RaiseEvent(new TokenUsed(this.Value, this.Type));
        }
        #endregion

        #region inner event handlers
        void IEventHandler<TokenUsed>.Handle(TokenUsed @event)
        {
            this.IsUsed = true;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion
    }
}
