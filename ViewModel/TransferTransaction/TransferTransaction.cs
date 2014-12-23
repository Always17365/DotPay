using DotPay.Common;
using DotPay.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;

namespace DotPay.ViewModel
{
    public class TransferTransaction
    {
        public int ID { get; set; }
        public string TxId { get; set; }
        public string SequenceNo { get; set; }
        public string TransferNo { get; set; }
        public string CNSourcePayway { get { return this.SourcePayway.GetDescription(); } }
        public PayWay SourcePayway { get; set; }
        public string CNState { get { return this.State.GetDescription(); } }
        public TransactionState State { get; set; }            
        public string Account { get; set; }
        public decimal Amount { get; set; }
        public string PayWay { get; set; }
        public int CreateAt { get; set; } 
    }
}
