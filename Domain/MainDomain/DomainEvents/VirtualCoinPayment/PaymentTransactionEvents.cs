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
    public class PaymentTransactionCreated : DomainEvent
    {
        //user id 为兼容恒星币人工充值，以后开通自动就无保留必要了
        public PaymentTransactionCreated(string txid, string address, CurrencyType currency, decimal amount, ReceivePaymentTransaction ptxEntity, int userID = 0)
        {
            this.UserID = userID;
            this.PtxEntity = ptxEntity;
            this.TxID = txid;
            this.Currency = currency;
            this.Address = address;
            this.Amount = amount;
        }
        public int UserID { get; private set; }
        public string TxID { get; private set; }
        public CurrencyType Currency { get; private set; }
        public string Address { get; private set; }
        public decimal Amount { get; private set; }
        public ReceivePaymentTransaction PtxEntity  { get; private set; }
    }

    public class PaymentTransactionConfirmed : DomainEvent
    {
        public PaymentTransactionConfirmed(int paymentTransactionID,string txID, int confirmations, CurrencyType currencyType, int byUserID = 0)
        {
            this.PaymentTransactionID = paymentTransactionID;
            this.TxID = txID;
            this.Confirmations = confirmations;
            this.Currency = currencyType;
            this.ByUserID = byUserID;
        }
        public int PaymentTransactionID { get; private set; }
        public string TxID { get; private set; }
        public CurrencyType Currency { get; private set; }
        public int Confirmations { get; private set; }
        public int ByUserID { get; private set; }
    }
}
