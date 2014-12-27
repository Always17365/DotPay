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
        public InboundTransferToThirdPartyPaymentTxCreated(string txid, string account, PayWay payway, decimal amount, PayWay sourcePayway, string realName, string memo)
        {
            this.TxId = txid;
            this.Account = account;
            this.Amount = amount;
            this.PayWay = payway;
            this.SourcePayway = sourcePayway;
            this.Memo = memo;
            this.RealName = realName;
        }
        public string TxId { get; private set; }
        public string Account { get; private set; }
        public PayWay PayWay { get; private set; }
        public decimal Amount { get; private set; }
        public PayWay SourcePayway { get; private set; }
        public string Memo { get; private set; }
        public string RealName { get; private set; }
    }

    public class InboundTransferToThirdPartyPaymentTxMarkProcessing : DomainEvent
    {
        public InboundTransferToThirdPartyPaymentTxMarkProcessing(int transferID, PayWay payway, int byUserID)
        {
            this.TransferID = transferID;
            this.PayWay = payway;
            this.ByUserID = byUserID;
        }
        public int TransferID { get; private set; }
        public PayWay PayWay { get; private set; }
        public int ByUserID { get; private set; }
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
