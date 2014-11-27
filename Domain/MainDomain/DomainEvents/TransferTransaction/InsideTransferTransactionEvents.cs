using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{
    public class InsideTransferTransactionCreated : DomainEvent
    {
        public InsideTransferTransactionCreated(int fromUserID, int toUserID, CurrencyType currency, decimal amount, PayWay payway, string description, InsideTransferTransaction tx)
        {
            this.FromUserID = fromUserID;
            this.ToUserID = toUserID;
            this.Currency = currency;
            this.Amount = amount;
            this.PayWay = payway;
            this.Description = description;
            this.TransactionEntity = tx;
        }
        public int FromUserID { get; private set; }
        public int ToUserID { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public PayWay PayWay { get; private set; }
        public string Description { get; private set; }
        public InsideTransferTransaction TransactionEntity { get; private set; }
    }

    public class InsideTransferTransactionComplete : DomainEvent
    {
        public InsideTransferTransactionComplete(int internalTransferID, CurrencyType currency)
        {
            this.InternalTransferID = internalTransferID;
            this.Currency = currency;
        }
        public int InternalTransferID { get; private set; }
        public CurrencyType Currency { get; private set; }
    }

    public class InsideTransferTransactionConfirmed : DomainEvent
    {
        public InsideTransferTransactionConfirmed(int internalTransferID, CurrencyType currency)
        {
            this.InternalTransferID = internalTransferID;
            this.Currency = currency;
        }
        public int InternalTransferID { get; private set; }
        public CurrencyType Currency { get; private set; }
    }
}
