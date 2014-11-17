using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.ViewModel
{
    [Serializable]
    public class InsideTransferModel
    {
        public int ID { get; set; }
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public string SequenceNo { get; set; }
        public int CreateAt { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Payway { get; set; } 
        public CurrencyType Currency { get; set; } 
    }
}
