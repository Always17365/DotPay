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
    public class DepositCodeListModel
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
        public int LockExpirationTime { get; set; }
        public bool IsLocked { get; set; }
        public bool IsUsed { get; set; }
        public int LockBy { get; set; }
        public string BankAccount { get; set; }
        public string TransferTxNo { get; set; }
        public decimal FundSourceAmount { get; set; }
        public int CreateAt { get; set; }
        public int UsedAt { get; set; }
        public int CreateBy { get; set; }
        

    }

}
