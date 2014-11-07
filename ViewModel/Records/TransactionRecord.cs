using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.ViewModel
{
    [Serializable]
    public class TransactionRecord
    {
        public string SequenceNo { get; set; }
        public int Date { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal Income { get; set; }
        public decimal Output { get; set; }
        public string Payway { get; set; }
        public decimal Balance { get; set; }
        public CurrencyType Currency { get; set; }
        public string TradeNo { get; set; }
    }
}
