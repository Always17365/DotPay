using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.EventSourcing;
using Dotpay.Common;
using Dotpay.Actor;
using Dotpay.Common.Enum;
using Orleans.Concurrency;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class RippleToFIInitializedEvent : GrainEvent
    {
        public RippleToFIInitializedEvent(long transactionId, string rippleTxId, string invoiceId, Payway payway,string destination, CurrencyType currency,decimal amount, decimal sendAmount, string memo)
        {
            this.TransactionId = transactionId;
            this.RippleTxId = rippleTxId;
            this.InvoiceId = invoiceId;
            this.Payway = payway;
            this.Destination = destination;
            this.Currency = currency;
            this.Amount = amount;
            this.SendAmount = sendAmount;
            this.Memo = memo;
        }
        public long TransactionId { get; private set; }
        public string RippleTxId { get; private set; }
        public string InvoiceId { get; private set; }
        public Payway Payway { get; private set; }
        public string Destination { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public decimal SendAmount { get; private set; }
        public string Memo { get; private set; }
    }
    public class RippleToFILockedEvent : GrainEvent
    {
        public RippleToFILockedEvent(Guid managerId)
        {
            this.ManagerId = managerId;
        }

        public Guid ManagerId { get; private set; }
    }

    public class RippleToFICompletedEvent : GrainEvent
    {
        public RippleToFICompletedEvent(string transferNo, Guid managerId, string managerMemo)
        {
            this.TransferNo = transferNo;
            this.ManagerId = managerId;
            this.ManagerMemo = managerMemo;
        }

        public string TransferNo { get; private set; }
        public Guid ManagerId { get; private set; }
        public string ManagerMemo { get; private set; }
    }
 public class RippleToFIFailedEvent : GrainEvent
    {
     public RippleToFIFailedEvent(string reason, Guid managerId)
        {
            this.Reason = reason;
            this.ManagerId = managerId; 
        }

        public string Reason { get; private set; }
        public Guid ManagerId { get; private set; } 
    }
}
