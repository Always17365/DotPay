using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.Common;
using FC.Framework;

namespace DotPay.RippleDomain.Events
{
    public class RippleDepositCreated : DomainEvent
    {
        #region ctor
        public RippleDepositCreated(string cnyDepositSeqNo, CurrencyType currency, decimal amount)
        {
            this.CNYDepositSeq = cnyDepositSeqNo;
            this.Currency = currency;
            this.Amount = amount;
        }

        #endregion

        public virtual string CNYDepositSeq { get; protected set; }
        public virtual CurrencyType Currency { get; protected set; }
        public virtual decimal Amount { get; protected set; }
    }
}
