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
    public abstract class VirtualCoinWithdraw : DomainBase, IVirtualCoinWithdraw, IAggregateRoot,
                               IEventHandler<VirtualCoinWithdrawCreated>,
                               IEventHandler<VirtualCoinWithdrawSetFee>,
                               IEventHandler<VirtualCoinWithdrawSkipVerify>,
                               IEventHandler<VirtualCoinWithdrawVerified>,
                               IEventHandler<VirtualCoinWithdrawCanceled>,
                               IEventHandler<VirtualCoinWithdrawCompleted>,
                               IEventHandler<VirtualCoinWithdrawTranferFailed>
    {
        #region ctor
        protected VirtualCoinWithdraw() { }
        public VirtualCoinWithdraw(CurrencyType currency, int withdrawUserID, int accountID, decimal amount, string receiveAddress)
        {
            this.RaiseEvent(new VirtualCoinWithdrawCreated(withdrawUserID, accountID, amount, receiveAddress, currency, this));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string UniqueID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual int AccountID { get; protected set; }
        public virtual string TxID { get; protected set; }
        public virtual WithdrawState State { get; protected set; }
        protected virtual IWithdrawStateMachine StateMachine { get { return WithdrawStateMachineFactory.CreateStateMachine(this); } }
        public virtual decimal Amount { get; protected set; }
        public virtual decimal Fee { get; protected set; }
        public virtual decimal TxFee { get; protected set; }
        public virtual string ReceiveAddress { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int VerifyAt { get; protected set; }
        public virtual int DoneAt { get; protected set; }
        public virtual int FailAt { get; protected set; }
        public virtual string Memo { get; protected set; }
        #endregion

        #region public methods
        public abstract void SetFee(decimal fee);

        public abstract void Verify(int byUserID, string memo);

        public abstract void SkipVerify();

        public abstract void Complete(string txID, decimal txfee);

        public abstract void Cancel(int byUserID, string memo);
        public abstract void MarkTransferFail(int byUserID);

        #endregion

        #region inner events handler
        void IEventHandler<VirtualCoinWithdrawCreated>.Handle(VirtualCoinWithdrawCreated @event)
        {
            this.UserID = @event.WithdrawUserID;
            this.UniqueID = Guid.NewGuid().Shrink();
            this.AccountID = @event.AccountID;
            this.Amount = @event.Amount;
            this.TxID = string.Empty;
            this.ReceiveAddress = @event.ReceiveAddress;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.State = WithdrawState.WaitVerify;
            this.Memo = string.Empty;
        }

        void IEventHandler<VirtualCoinWithdrawSetFee>.Handle(VirtualCoinWithdrawSetFee @event)
        {
            this.Fee = @event.Fee;
        }

        void IEventHandler<VirtualCoinWithdrawSkipVerify>.Handle(VirtualCoinWithdrawSkipVerify @event)
        {
            this.VerifyAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.State = WithdrawState.Processing;
        }

        void IEventHandler<VirtualCoinWithdrawVerified>.Handle(VirtualCoinWithdrawVerified @event)
        {
            this.VerifyAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.State = WithdrawState.Processing;
        }
        void IEventHandler<VirtualCoinWithdrawCanceled>.Handle(VirtualCoinWithdrawCanceled @event)
        {
            this.State = WithdrawState.Cancel;
            this.Memo = @event.Memo;
        }

        void IEventHandler<VirtualCoinWithdrawCompleted>.Handle(VirtualCoinWithdrawCompleted @event)
        {
            this.TxID = @event.TxID;
            this.TxFee = @event.TxFee;
            this.State = WithdrawState.Complete;
            this.DoneAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        void IEventHandler<VirtualCoinWithdrawTranferFailed>.Handle(VirtualCoinWithdrawTranferFailed @event)
        {
            this.State = WithdrawState.Fail;
            this.FailAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion
    }
}
