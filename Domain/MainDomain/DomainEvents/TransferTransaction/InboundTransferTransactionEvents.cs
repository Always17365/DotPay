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
    public class InboundTransferToThirdPartyPaymentTxCreated : DomainEvent
    {
        public InboundTransferToThirdPartyPaymentTxCreated(string account, PayWay payway, decimal amount)
        {
            this.Account = account;
            this.Amount = amount;
            this.PayWay = payway;
        }
        public string Account { get; private set; }
        public PayWay PayWay { get; private set; }
        public decimal Amount { get; private set; }
    }

    public class InboundTransferToThirdPartyPaymentTxComplete : DomainEvent
    {
        public InboundTransferToThirdPartyPaymentTxComplete(int transferID, PayWay payway, string transferNo, int byUserID)
        {
            this.TransferID = transferID;
            this.PayWay = payway;
            this.TransferNo = transferNo;
            this.ByUserID = byUserID;
        }
        public int TransferID { get; private set; }
        public PayWay PayWay { get; private set; }
        public string TransferNo { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class InboundTransferToThirdPartyPaymentTxFailed : DomainEvent
    {
        public InboundTransferToThirdPartyPaymentTxFailed(int transferID, PayWay payway, string reason, int byUserID)
        {
            this.TransferID = transferID;
            this.PayWay = payway;
            this.Reason = reason;
            this.ByUserID = byUserID;
        }
        public int TransferID { get; private set; }
        public PayWay PayWay { get; private set; }
        public string Reason { get; private set; }
        public int ByUserID { get; private set; }
    }
}
