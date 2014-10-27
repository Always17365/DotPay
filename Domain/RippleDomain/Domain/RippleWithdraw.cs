using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.RippleDomain
{
    public class RippleWithdraw : DomainBase
    {
        public virtual int ID { get; protected set; }
        public virtual string TxID { get; protected set; }
        public virtual CurrencyType Currency { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual TransactionState State { get; protected set; }
    }
}
