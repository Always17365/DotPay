using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.Domain.Events;

namespace DotPay.Domain
{
    public class WithdrawReceiverBankAccount : DomainBase, IAggregateRoot,
                                   IEventHandler<UserBankAccountCreated>,
                                   IEventHandler<UserBankAccountMarkAsValid>,
                                   IEventHandler<UserBankAccountMarkAsInvalid>
    {
        #region ctor
        protected WithdrawReceiverBankAccount() { }
        public WithdrawReceiverBankAccount(int userID, Bank bank,  string bankAccount, string receiverName)
        {
            this.RaiseEvent(new UserBankAccountCreated(userID,bank, bankAccount, receiverName));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual Bank Bank { get; protected set; }
        public virtual ValidState Valid { get; protected set; }
        public virtual string BankAccount { get; protected set; }
        public virtual string ReceiverName { get; protected set; }
        //public virtual int BankOutletsID { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int MarkBy { get; protected set; }
        public virtual int MarkAt { get; protected set; }
        #endregion

        #region public methods
        public virtual void MarkAsValid(int userID)
        {
            this.RaiseEvent(new UserBankAccountMarkAsValid(this.ID, userID));
        }

        public virtual void MarkAsInvalid(int userID)
        {
            this.RaiseEvent(new UserBankAccountMarkAsInvalid(this.ID, userID));
        }
        #endregion

        #region inner events handlers
        void IEventHandler<UserBankAccountCreated>.Handle(UserBankAccountCreated @event)
        {
            this.UserID = @event.BankAccountOwnerID;
            this.Valid = ValidState.None;
            this.BankAccount = @event.BankAccount;
            this.ReceiverName = @event.ReceiverName;
            this.Bank = @event.Bank;
            //this.BankOutletsID = @event.BankID;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<UserBankAccountMarkAsValid>.Handle(UserBankAccountMarkAsValid @event)
        {
            this.Valid = ValidState.Valid;
            this.MarkBy = @event.MarkBy;
            this.MarkAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<UserBankAccountMarkAsInvalid>.Handle(UserBankAccountMarkAsInvalid @event)
        {
            this.Valid = ValidState.Invalid;
            this.MarkBy = @event.MarkBy;
            this.MarkAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion
    }
}
