using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.ViewModel
{
    [Serializable]
    public class OutboundTransferModel
    {
        public int ID { get; set; }
        public int FromUserID { get; set; }
        public string Destination { get; set; }
        public string SequenceNo { get; set; } 
        public string CNPayway { get; set; }
        public virtual decimal SourceAmount { get; protected set; }
        public virtual CurrencyType TargetCurrency { get; protected set; }
        public virtual decimal TargetAmount { get; protected set; }
        public virtual PayWay PayWay { get; protected set; }
        public virtual TransactionState State { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual string Memo { get; protected set; }
    }
}
