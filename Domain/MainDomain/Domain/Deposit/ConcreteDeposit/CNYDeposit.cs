using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.MainDomain.Exceptions;

namespace DotPay.MainDomain
{
    public class CNYDeposit : DomainBase, ICNYDeposit, IAggregateRoot,
                              IEventHandler<CNYDepositCreated>,
                              IEventHandler<CNYDepositCompleted>,
                              IEventHandler<CNYDepositUndoComplete>
    {
        #region ctor
        protected CNYDeposit() { }

        public CNYDeposit(int userID, int accountID, decimal amount)
        {
            this.RaiseEvent(new CNYDepositCreated(userID, accountID, amount, this));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string SequenceNo { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual int AccountID { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual PayWay PayWay { get; protected set; }
        public virtual DepositState State { get; protected set; }
        protected virtual IDepositStateMachine StateMachine { get { return DepositStateMachineFactory.CreateStateMachine(this); } }
        public virtual int CreateAt { get; protected set; }
        public virtual int DoneAt { get; protected set; }
        public virtual string Memo { get; protected set; }
        #endregion

        #region IMPL ICNYDeposit

        public virtual void VerifyAmount(int byUserID, PayWay payway, Bank bank, int fundSourceID, decimal fundAmount)
        {
            if (fundAmount != this.Amount)
                throw new DepositNotEqualsFundAmountException();

            this.StateMachine.VerifyForCNY(byUserID, payway, bank, fundSourceID, fundAmount);
        }

        public virtual void Complete(int byUserID)
        {
            this.StateMachine.CompleteForCNY(byUserID);
        }

        public virtual void UndoComplete(int byUserID)
        {
            this.StateMachine.UndoCompleteForCNY(byUserID);
        }
        #endregion

        #region inner event handlers

        void IEventHandler<CNYDepositCreated>.Handle(CNYDepositCreated @event)
        {
            this.UserID = @event.UserID;
            this.SequenceNo = DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().Shrink().Replace("-", string.Empty).Replace("_", string.Empty);
            this.AccountID = @event.AccountID;
            this.Amount = @event.DepositAmount;
            this.State = DepositState.Pending;
            this.Memo = string.Empty;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<CNYDepositCompleted>.Handle(CNYDepositCompleted @event)
        {
            this.State = DepositState.Complete;
            this.DoneAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<CNYDepositUndoComplete>.Handle(CNYDepositUndoComplete @event)
        {
            this.State = DepositState.Verify;
            this.DoneAt = 0;
        }

        #endregion
    }
}
