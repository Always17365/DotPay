using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotpay.TaobaoMonitor
{
    internal class TaobaoAutoDeposit
    {
        public long tid { get; set; }
        public int amount { get; set; }
        public bool has_buyer_message { get; set; }
        public string taobao_status { get; set; }
        public string ripple_address { get; set; }
        public RippleTransactionStatus ripple_status { get; set; }
        public string txid { get; set; }
        public int tx_lastLedgerSequence { get; set; }
        public int retry_Counter { get; set; }
        public DateTime? first_submit_at { get; set; }
        public string memo { get; set; }
    }
}
