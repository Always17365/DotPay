using DotPay.Common;
using FC.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.RippleCommand
{
    public class CreateInboundTx : FC.Framework.Command
    {
        public CreateInboundTx(string txid, int destinationTag, decimal amount)
        {
            Check.Argument.IsNotEmpty(txid, "txid");
            Check.Argument.IsNotNegativeOrZero(destinationTag, "destinationTag");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");

            this.TxID = txid;
            this.DestinationTag = destinationTag;
            this.Amount = amount;
        }
        public string TxID { get; protected set; }
        public int DestinationTag { get; protected set; }
        public decimal Amount { get; protected set; }
    }

    public class CreateThirdPartyPaymentInboundTx : FC.Framework.Command
    {
        public CreateThirdPartyPaymentInboundTx(PayWay payway, string tpp_account)
        {
            Check.Argument.IsNotEmpty(tpp_account, "tpp_account");
            Check.Argument.IsNotNegativeOrZero((int)payway, "payway");

            this.PayWay = payway;
            this.Destination = tpp_account;
        }
        public CreateThirdPartyPaymentInboundTx(PayWay payway, string tpp_account, string realName, decimal amount, string memo)
            : this(payway, tpp_account)
        {
            Check.Argument.IsNotNegativeOrZero(amount, "amount");

            this.RealName = realName;
            this.Amount = amount;
            this.Memo = memo;
        }
        public PayWay PayWay { get; protected set; }
        public string Destination { get; protected set; }
        public string RealName { get; protected set; }
        public decimal Amount { get; protected set; }
        public string Memo { get; protected set; }


        public string ResultInvoiceID { get; set; }
        public int ResultDestinationTag { get; set; }
    }
    public class CompleteThirdPartyPaymentInboundTx : FC.Framework.Command
    {
        public CompleteThirdPartyPaymentInboundTx(string txid, int destinationtag, decimal amount, string invoiceId = "")
        {
            Check.Argument.IsNotEmpty(txid, "txid");
            Check.Argument.IsNotNegativeOrZero(destinationtag, "destinationtag");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");

            this.InvoiceId = invoiceId;
            this.TxId = txid;
            this.DestinationTag = destinationtag;
            this.Amount = amount;
        }
        public string InvoiceId { get; protected set; }
        public string TxId { get; protected set; }
        public int DestinationTag { get; set; }
        public decimal Amount { get; set; }
    }

    //public class CompleteThirdPartyPaymentInboundTx : FC.Framework.Command
    //{
    //    public CompleteThirdPartyPaymentInboundTx(PayWay payway, string txid, int destinationtag, decimal amount)
    //    {
    //        this.PayWay = payway;
    //        this.TxId = txid;
    //        this.DestinationTag = destinationtag;
    //        this.Amount = amount;
    //    }
    //    public PayWay PayWay { get; protected set; }
    //    public string TxId { get; protected set; }
    //    public int DestinationTag { get; set; }
    //    public decimal Amount { get; set; }
    //}
}
