using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.ViewModel
{
    [Serializable]
    public class TaobaoRippleDeposit
    {
        public long Tid { get; set; }
        public string buyer_nick { get; set; }
        public DateTime pay_time { get; set; }
        public decimal amount { get; set; }
        public bool has_buyer_message { get; set; }
        public string taobao_status { get; set; }
        public string ripple_address { get; set; }
        public RippleTransactionStatus ripple_status { get; set; }
        public string txid { get; set; }
        public long tx_lastLedgerSequence { get; set; }
        public string memo { get; set; } 
    }

    public enum RippleTransactionStatus
    {
        Init = 0,
        Pending = 1,
        Submited = 2,
        Successed = 3,
        Failed = 4
    }
}
