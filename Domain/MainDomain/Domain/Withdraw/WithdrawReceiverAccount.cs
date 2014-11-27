using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.MainDomain.Events;

namespace DotPay.MainDomain
{
    public class WithdrawReceiverAccount : DomainBase, IAggregateRoot,
                                   IEventHandler<UserReceiverAccountCreated>,
                                   IEventHandler<UserReceiverAccountMarkAsValid>,
                                   IEventHandler<UserReceiverAccountMarkAsInvalid>
    {
        #region ctor
        protected WithdrawReceiverAccount() { }
        public WithdrawReceiverAccount(int userID, PayWay payway, string bankAccount, string receiverName)
        {
            this.RaiseEvent(new UserReceiverAccountCreated(userID, payway, bankAccount, receiverName));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual PayWay PayWay { get; protected set; }
        public virtual ValidState Valid { get; protected set; }
        public virtual string BankAccount { get; protected set; }
        public virtual string ReceiverName { get; protected set; } 
        public virtual int CreateAt { get; protected set; }
        public virtual int MarkBy { get; protected set; }
        public virtual int MarkAt { get; protected set; }
        #endregion

        #region public methods
        public virtual void MarkAsValid(int userID)
        {
            this.RaiseEvent(new UserReceiverAccountMarkAsValid(this.ID, userID));
        }

        public virtual void MarkAsInvalid(int userID)
        {
            this.RaiseEvent(new UserReceiverAccountMarkAsInvalid(this.ID, userID));
        }
        #endregion

        #region inner events handlers
        void IEventHandler<UserReceiverAccountCreated>.Handle(UserReceiverAccountCreated @event)
        {
            this.UserID = @event.BankAccountOwnerID;
            this.Valid = ValidState.None;
            this.BankAccount = @event.BankAccount;
            this.ReceiverName = @event.ReceiverName;
            this.PayWay = @event.PayWay;
            //this.BankOutletsID = @event.BankID;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<UserReceiverAccountMarkAsValid>.Handle(UserReceiverAccountMarkAsValid @event)
        {
            this.Valid = ValidState.Valid;
            this.MarkBy = @event.MarkBy;
            this.MarkAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<UserReceiverAccountMarkAsInvalid>.Handle(UserReceiverAccountMarkAsInvalid @event)
        {
            this.Valid = ValidState.Invalid;
            this.MarkBy = @event.MarkBy;
            this.MarkAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion
    }
}
