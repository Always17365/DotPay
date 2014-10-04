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
    public class CNYWithdraw : DomainBase, IAggregateRoot, ICNYWithdraw,
                               IEventHandler<CNYWithdrawCreated>,
                               IEventHandler<CNYWithdrawSetFee>,
                               IEventHandler<CNYWithdrawVerified>,
                               IEventHandler<CNYWithdrawSkipVerify>,
                               IEventHandler<CNYWithdrawSubmitedToProcess>,
                               IEventHandler<CNYWithdrawTransferFailed>,
                               IEventHandler<CNYWithdrawModifiedReceiverBankAccount>,
                               IEventHandler<CNYWithdrawCompleted>,
                               IEventHandler<CNYWithdrawCanceled>
    {
        #region ctor
        protected CNYWithdraw() { }
        public CNYWithdraw(int withdrawUserID, int accountID, decimal amount, int userBankAccountID)
        {
            this.RaiseEvent(new CNYWithdrawCreated(withdrawUserID, accountID, amount, userBankAccountID, this));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual string UniqueID { get; protected set; }
        public virtual int AccountID { get; protected set; }
        public virtual int TransferAccountID { get; protected set; }
        public virtual string TransferNo { get; protected set; }
        public virtual int ReceiverBankAccountID { get; protected set; }
        public virtual WithdrawState State { get; protected set; }
        protected virtual IWithdrawStateMachine StateMachine { get { return WithdrawStateMachineFactory.CreateStateMachine(this); } }
        public virtual decimal Amount { get; protected set; }
        public virtual decimal Fee { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int SubmitAt { get; protected set; }
        public virtual int VerifyAt { get; protected set; }
        public virtual int DoneAt { get; protected set; }
        public virtual string Memo { get; protected set; }
        #endregion

        #region public methods
        public virtual void SetFee(decimal fee)
        {
            this.StateMachine.SetFee(CurrencyType.CNY, fee);
        }

        public virtual void Verify(int byUserID, string memo)
        {
            this.StateMachine.VerifyForCNY(byUserID, memo);
        }

        public virtual void SkipVerify()
        {
            this.StateMachine.SkipVerifyForCNY();
        }

        public virtual void ModifyReceiverBankAccount(int bankAccountID, int byUserID)
        {
            this.StateMachine.ModifyReceiverBankAccountID(bankAccountID, byUserID);
        }

        public virtual void SubmitToProcess(int byUserID)
        {
            this.StateMachine.SubmitToProcess(byUserID);
        }

        public virtual void TranferFail(string memo, int byUserID)
        {
            this.StateMachine.TranferFail(byUserID, memo);
        }

        public virtual void Complete(int transferAccountID, string transferNo, int byUserID)
        {
            this.StateMachine.CompleteForCNY(transferAccountID, transferNo, byUserID);
        }

        public virtual void Cancel(string memo, int byUserID)
        {
            this.StateMachine.CancelForCNY(byUserID, memo);
        }

        #endregion

        #region inner events handlers
        void IEventHandler<CNYWithdrawCreated>.Handle(CNYWithdrawCreated @event)
        {
            this.UserID = @event.WithdrawUserID;
            this.UniqueID = Guid.NewGuid().Shrink();
            this.AccountID = @event.AccountID;
            this.ReceiverBankAccountID = @event.UserBankAccountID;
            this.Amount = @event.Amount;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.TransferNo = string.Empty;
            this.State = WithdrawState.WaitVerify;
            this.Memo = string.Empty;
        }

        void IEventHandler<CNYWithdrawSetFee>.Handle(CNYWithdrawSetFee @event)
        {
            this.Fee = @event.Fee;
        }

        void IEventHandler<CNYWithdrawSkipVerify>.Handle(CNYWithdrawSkipVerify @event)
        {
            this.State = WithdrawState.WaitSubmit;
            this.VerifyAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<CNYWithdrawVerified>.Handle(CNYWithdrawVerified @event)
        {
            this.State = WithdrawState.WaitSubmit;
            this.Memo += @event.Memo;
            this.VerifyAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<CNYWithdrawSubmitedToProcess>.Handle(CNYWithdrawSubmitedToProcess @event)
        {
            this.SubmitAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.State = WithdrawState.Processing;
        }

        void IEventHandler<CNYWithdrawCompleted>.Handle(CNYWithdrawCompleted @event)
        {
            this.State = WithdrawState.Complete;
            this.TransferAccountID = @event.TransferAccountID;
            this.TransferNo = @event.TransferNo;
            this.DoneAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<CNYWithdrawTransferFailed>.Handle(CNYWithdrawTransferFailed @event)
        {
            this.State = WithdrawState.Fail;
            this.Memo += @event.Memo;
        }

        void IEventHandler<CNYWithdrawModifiedReceiverBankAccount>.Handle(CNYWithdrawModifiedReceiverBankAccount @event)
        {
            this.ReceiverBankAccountID = @event.ReceiverBankAccountID;
            this.State = WithdrawState.Processing;
        }

        void IEventHandler<CNYWithdrawCanceled>.Handle(CNYWithdrawCanceled @event)
        {
            this.State = WithdrawState.Cancel;
        }
        #endregion
    }
}
