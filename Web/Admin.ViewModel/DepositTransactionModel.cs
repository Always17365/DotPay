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
    public class DepositListViewModel
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string SequenceNo { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public DepositStatus Status { get; set; }
        public Payway Payway { get; set; }
        public string TransactionNo { get; set; }
        public string Memo { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? FailAt { get; set; }
        public Guid? ManagerId { get; set; }
        public string FailReason { get; set; }
    }
}
