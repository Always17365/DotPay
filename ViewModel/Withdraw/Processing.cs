using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DotPay.Common;
using FC.Framework;

namespace DotPay.ViewModel
{ 
    public class VirtualCoinWithdrawListModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string UniqueID { get; set; }
        public string TxID { get; set; }            
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string ReceiveAddress { get; set; }
        public decimal TxFee { get; set; }
        public int CreateAt { get; set; }
        public string Memo { get; set; }

    }


}
