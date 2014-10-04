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
    public class WithdrawListModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string CNBank { get { return Language.LangHelpers.Lang(this.Bank.GetDescription()); } }
        public string CNPayWay { get { return Language.LangHelpers.Lang(this.PayWay.GetDescription()); } }
        public PayWay PayWay { get; set; }
        public Bank Bank { get; set; }
        public WithdrawState State { get; set; }
        public bool DepositCodeIsUsed { get; set; }
        public string BankAccount { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiveAddress { get; set; }
        public string TxID { get; set; }
        public string TransferNo { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public int CreateAt { get; set; }

    }
    public class WithdraweCreateModel
    {
        public int CapitalAccountID { get; set; }
        public Bank Bank { get; set; }
        public string TransferTxNo { get; set; }
        public PayWay Payway { get; set; }
        public decimal Amount { get; set; }
        public string Extra { get; set; }
        public int CreateBy { get; set; }
    }

}
