using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.MainDomain.Events;
using FC.Framework;

namespace DotPay.MainDomain
{
    public abstract class AccountVersion : DomainBase, IAggregateRoot,
                                  IEventHandler<AccountVersionCreated>
    {
        #region ctor
        protected AccountVersion() { }

        public AccountVersion(int userID, int accountID, decimal amount,
                              decimal balance, decimal locked, decimal @in, decimal @out,
                              int modifyID, int modifyType)
        {
            this.RaiseEvent(new AccountVersionCreated(userID, accountID,
                                amount, balance, locked, @in, @out, modifyID, modifyType));

        }
        public AccountVersion(int userID, int accountID, decimal amount,
                            decimal balance, decimal locked, decimal @in, decimal @out,
                            string modifyUniqueID, int modifyType)
        {
            this.RaiseEvent(new AccountVersionCreated(userID, accountID,
                                amount, balance, locked, @in, @out, modifyUniqueID, modifyType));

        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual int AccountID { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual decimal Balance { get; protected set; }
        public virtual decimal Locked { get; protected set; }
        public virtual decimal In { get; protected set; }
        public virtual decimal Out { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int ModifyID { get; protected set; }
        public virtual string ModifyUniqueID { get; protected set; }
        public virtual int ModifyType { get; protected set; } 
        #endregion

        void IEventHandler<AccountVersionCreated>.Handle(AccountVersionCreated @event)
        {
            this.UserID = @event.UserID;
            this.AccountID = @event.AccountID;
            this.Amount = @event.Amount;
            this.Balance = @event.Balance;
            this.Locked = @event.Locked;
            this.In = @event.AmountIn;
            this.Out = @event.AmountOut;
            this.ModifyID = @event.ModifyID;
            this.ModifyUniqueID = @event.ModifyUniqueID;
            this.ModifyType = @event.ModifyType;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
    }
}
