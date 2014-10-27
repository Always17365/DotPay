using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.Common;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.RippleDomain.Events;

namespace DotPay.RippleDomain
{
    public class RippleDeposit : DomainBase, IEventHandler<RippleDepositCreated>
    {
        #region ctor
        protected RippleDeposit() { }

        public RippleDeposit(string cnyDepositSeqNo, CurrencyType currency, decimal amount)
        {
            this.RaiseEvent(new RippleDepositCreated(cnyDepositSeqNo, currency, amount));
        }

        #endregion

        public virtual int ID { get; protected set; }
        public virtual string CNYDepositSeq { get; protected set; }
        public virtual string TxID { get; protected set; }
        public virtual CurrencyType Currency { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual TransactionState State { get; protected set; }

        #region Event handler
        void IEventHandler<RippleDepositCreated>.Handle(RippleDepositCreated @event)
        {
            this.CNYDepositSeq = @event.CNYDepositSeq;
            this.Currency = @event.Currency;
            this.Amount = @event.Amount;
            this.State = TransactionState.Init;
            this.TxID = string.Empty;
        }
        #endregion
    }
}
