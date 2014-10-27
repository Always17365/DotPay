using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.RippleDomain.Events;
using DotPay.RippleDomain.Exceptions;

namespace DotPay.RippleDomain
{
    public class RippleActiveTx : DomainBase, IAggregateRoot, IEventHandler<RippleAciveTxCreated>,
                                  IEventHandler<RippleAciveTxCompelted>,
                                  IEventHandler<RippleAciveTxFailed>
    {
        #region ctor
        protected RippleActiveTx() { }

        public RippleActiveTx(string txhash, string txblob, string source, string destination, decimal amount, decimal fee)
        {
            this.RaiseEvent(new RippleAciveTxCreated(txhash, txblob, source, destination, amount, fee));
        }

        #endregion
        #region properties
        public virtual int ID { get; protected set; }
        public virtual string TxID { get; protected set; }
        public virtual string TxBlob { get; protected set; }
        public virtual string Source { get; protected set; }
        public virtual string Destination { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual decimal Fee { get; protected set; }
        public virtual TransactionState State { get; protected set; }
        public virtual string Memo { get; set; }
        public virtual int CreateAt { get; set; }
        public virtual int LastUpdateAt { get; set; } 
        #endregion

        #region public method
        public virtual void Complete()
        {
            if (this.State != TransactionState.Pending)
                throw new RippleTransactionNotPendingException();
            else
            {
                this.RaiseEvent(new RippleAciveTxCompelted(this.ID));
            }
        }
        public virtual void MarkFail(string reason)
        {
            if (this.State != TransactionState.Pending)
                throw new RippleTransactionNotPendingException();
            else
            {
                this.RaiseEvent(new RippleAciveTxFailed(this.ID, reason));
            }
        }
        #endregion

        #region event handler
        void IEventHandler<RippleAciveTxCreated>.Handle(RippleAciveTxCreated @event)
        {
            this.TxID = @event.TxID;
            this.TxBlob = @event.TxBlob;
            this.Source = @event.Source;
            this.Destination = @event.Destination;
            this.Amount = @event.Amount;
            this.Fee = @event.Fee;
            this.State = TransactionState.Init;
            this.CreateAt = DateTime.Now.ToUnixTimestamp();
        }
        #endregion

        void IEventHandler<RippleAciveTxCompelted>.Handle(RippleAciveTxCompelted @event)
        {
            this.State = TransactionState.Success;
            this.LastUpdateAt = DateTime.Now.ToUnixTimestamp();
        }

        void IEventHandler<RippleAciveTxFailed>.Handle(RippleAciveTxFailed @event)
        {
            this.State = TransactionState.Fail;
            this.LastUpdateAt = DateTime.Now.ToUnixTimestamp();
        }
    }
}
