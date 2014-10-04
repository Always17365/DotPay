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
    public class WithdrawCheckPendingListModel
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string CreateAt { get; set; }
        public string Memo { get; set; }


    }
    public class DestroyListModel
    {
        public int ID { get; set; }
        public string UniqueID { get; set; }
        public int UserID { get; set; }
        public decimal Amount { get; set; }
        public string CreateAt { get; set; }
        public string Memo { get; set; }


    }
    public class CompleteListModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public decimal Amount { get; set; }
        public decimal TxFee { get; set; }
        public string CreateAt { get; set; }
        public string Memo { get; set; }
        public string UniqueID { get; set; }


    }


}
