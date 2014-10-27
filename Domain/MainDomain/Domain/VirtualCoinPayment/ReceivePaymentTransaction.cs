using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Exceptions;

namespace DotPay.MainDomain
{
    public abstract class ReceivePaymentTransaction : DomainBase, IAggregateRoot,
                                      IEventHandler<PaymentTransactionCreated>,
                                      IEventHandler<PaymentTransactionConfirmed>
    {
        #region ctor
        protected ReceivePaymentTransaction() { }
        public ReceivePaymentTransaction(string txid, string address, CurrencyType currency, decimal amount)
        {
            this.RaiseEvent(new PaymentTransactionCreated(txid, address, currency, amount, this));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual string UniqueID { get; protected set; }
        public virtual string TxID { get; protected set; }
        public virtual VirtualCoinTxState State { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual string Address { get; protected set; }
        public virtual int Confirmation { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int ConfirmAt { get; protected set; }
        #endregion

        #region public method
        public abstract void Confirm(int confirmations, string txid);
        #endregion

        #region inner event handlers
        void IEventHandler<PaymentTransactionCreated>.Handle(PaymentTransactionCreated @event)
        {
            this.TxID = @event.TxID;
            this.UniqueID = Guid.NewGuid().Shrink();
            this.Amount = @event.Amount.ToFixed(4);
            this.Address = @event.Address;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.State = VirtualCoinTxState.WaitConfirmation;
        }

        void IEventHandler<PaymentTransactionConfirmed>.Handle(PaymentTransactionConfirmed @event)
        {
            this.TxID = @event.TxID;
            this.Confirmation = @event.Confirmations;
            this.State = VirtualCoinTxState.Complete;
            this.ConfirmAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion
    }
}
