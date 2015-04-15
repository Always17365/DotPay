using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Common;
using Dotpay.Common.Enum;

namespace Dotpay.Admin.ViewModel
{
    [Serializable]
    public class TransferFromDotpayToFiListViewModel
    {
        public Guid Id { get; set; }
        public string UserLoginName { get; set; }
        public string SequenceNo { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Payway { get; set; }
        public string Bank { get; set; }
        public string RealName { get; set; }
        public string DestinationAccount { get; set; }
        public string TransactionNo { get; set; }
        public string Memo { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? FailAt { get; set; }
        DateTime? CompleteAt { get; set; }
        public string Manager  { get; set; }
        public string Reason { get; set; }
    }

    [Serializable]
    public class TransferToRippleListViewModel
    {
        public Guid Id { get; set; }
        public string Destination { get; set; }
        public string SequenceNo { get; set; }
        public string RippleTxId { get; set; }
        public RippleTransactionFailedType FailReason { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public TransferTransactionStatus Status { get; set; }
        public string Memo { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? FailAt { get; set; }
        DateTime? CompleteAt { get; set; }
    }

    [Serializable]
    public class InternalTransferListViewModel
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string SequenceNo { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public TransferTransactionStatus Status { get; set; }
        public string Memo { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? FailAt { get; set; }
        DateTime? CompleteAt { get; set; }
        public string Reason { get; set; }
    }
}
