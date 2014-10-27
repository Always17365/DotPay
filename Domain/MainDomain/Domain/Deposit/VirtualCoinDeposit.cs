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
    /// <summary>
    /// 所有虚拟币充值的基类，如果增加虚拟币，只需要声明一个新的虚拟币类，然后继承这个基类就可以了
    /// <remarks>注意增加mapper</remarks>
    /// </summary>
    public abstract class VirtualCoinDeposit : DomainBase, IVirtualCoinDeposit, IAggregateRoot,
                           IEventHandler<VirtualCoinDepositCreated>,
                           IEventHandler<VirtualCoinDepositVerified>,
                           IEventHandler<VirtualCoinDepositCompleted>
    {
        #region ctor
        protected VirtualCoinDeposit() { }

        public VirtualCoinDeposit(int userID, int accountID, string txid, CurrencyType currency, decimal amount, decimal fee, string memo)
        {
            this.RaiseEvent(new VirtualCoinDepositCreated(userID, accountID, txid, currency, amount, fee, memo, this));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string SequenceNo { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual int AccountID { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual decimal Fee { get; protected set; }
        public virtual string TxID { get; protected set; }
        public virtual DepositState State { get; protected set; }
        protected virtual IDepositStateMachine StateMachine { get { return DepositStateMachineFactory.CreateStateMachine(this); } }
        public virtual int CreateAt { get; protected set; }

        public virtual int VerifyAt { get; protected set; }
        public virtual int DoneAt { get; protected set; }
        public virtual string Memo { get; protected set; }
        #endregion

        #region public method

        public abstract void VerifyAmount(int byUserID, string txid, decimal txAmount);

        public abstract void Complete(CurrencyType currencyType, int byUserID);
        #endregion

        #region inner event handlers

        void IEventHandler<VirtualCoinDepositCreated>.Handle(VirtualCoinDepositCreated @event)
        {
            this.SequenceNo = Guid.NewGuid().Shrink();
            this.TxID = @event.TxID;
            this.UserID = @event.UserID;
            this.AccountID = @event.AccountID;
            this.Amount = @event.DepositAmount.ToFixed(4);
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.Fee = @event.DepositFee;
            this.State = DepositState.Pending;
            this.Memo = @event.Memo;
        }

        void IEventHandler<VirtualCoinDepositVerified>.Handle(VirtualCoinDepositVerified @event)
        {
            this.TxID = @event.TxID;
            this.State = DepositState.Verify;
            this.VerifyAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<VirtualCoinDepositCompleted>.Handle(VirtualCoinDepositCompleted @event)
        {
            this.State = DepositState.Complete;
            this.DoneAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion

    }
}