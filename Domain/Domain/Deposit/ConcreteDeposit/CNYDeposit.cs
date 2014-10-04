using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.Domain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.Domain.Exceptions;

namespace DotPay.Domain
{
    public class CNYDeposit : DomainBase, ICNYDeposit, IAggregateRoot,
                              IEventHandler<CNYDepositCreated>,
                              IEventHandler<CNYDepositSetFee>,
                              IEventHandler<CNYDepositVerified>,
                              IEventHandler<CNYDepositCompleted>,
                              IEventHandler<CNYDepositUndoComplete>
    {
        #region ctor
        protected CNYDeposit() { }

        public CNYDeposit(int userID, int accountID, decimal sumAmount, string memo, DepositType depositType)
        {
            this.RaiseEvent(new CNYDepositCreated(userID, accountID, sumAmount, memo, depositType, this));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string UniqueID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual int AccountID { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual PayWay PayWay { get; protected set; }
        public virtual decimal Fee { get; protected set; }
        public virtual int FundSourceID { get; protected set; }
        public virtual string FundExtra { get; protected set; }
        public virtual DepositState State { get; protected set; }
        protected virtual IDepositStateMachine StateMachine { get { return DepositStateMachineFactory.CreateStateMachine(this); } }
        public virtual DepositType Type { get; protected set; }
        public virtual int CreateAt { get; protected set; }

        public virtual int VerifyAt { get; protected set; }
        public virtual int DoneAt { get; protected set; }
        public virtual string Memo { get; protected set; }
        #endregion

        #region IMPL ICNYDeposit
        public virtual void SetAmountAndFee(decimal amount, decimal fee)
        {
            this.RaiseEvent(new CNYDepositSetFee(this, amount, fee));
        }

        public virtual void VerifyAmount(int byUserID, PayWay payway, Bank bank, int fundSourceID, decimal fundAmount, string fundExtra)
        {
            if (fundAmount != (this.Amount + this.Fee))
                throw new DepositNotEqualsFundAmountException();

            this.StateMachine.VerifyForCNY(byUserID, payway, bank, fundSourceID, fundAmount, fundExtra);
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
            this.UniqueID = Guid.NewGuid().Shrink();
            this.AccountID = @event.AccountID;
            this.Amount = @event.DepositSumAmount;
            this.State = DepositState.Pending;
            this.FundExtra = string.Empty;
            this.FundSourceID = 0;
            this.Memo = @event.Memo;
            this.Type = @event.DepositType;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<CNYDepositSetFee>.Handle(CNYDepositSetFee @event)
        {
            this.Fee = @event.Fee;
        }

        void IEventHandler<CNYDepositVerified>.Handle(CNYDepositVerified @event)
        {
            this.FundSourceID = @event.FundSourceID;
            this.FundExtra = @event.FundExtra;
            this.PayWay = @event.PayWay;
            this.State = DepositState.Verify;
            this.VerifyAt = @event.UTCTimestamp.ToUnixTimestamp();
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
