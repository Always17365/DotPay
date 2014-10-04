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
    public class FundSourceListModel
    {
        /*
        public int ID { get; set; }
        public Bank Bank { get; set; }
        public int CapitalAccountID { get; set; }
        public string TransferTxNo { get; set; }
        public string CNPayWay { get { return this.PayWay.GetDescription(); } }
        public PayWay PayWay { get; set; }
        public decimal Amount { get; set; }
        public string Extra { get; set; }
        public string CNState { get { return this.State.GetDescription(); } }
        public FundSourceState State { get; set; }
        public int CreateAt { get; set; }
        public int VerifyAt { get; set; }
        */
        public int ID { get; set; }
        public string TransferTxNo { get; set; }
        public string CNBank { get { return this.Bank.GetDescription(); } }
        public Bank Bank { get; set; }
        public string CNPayWay { get { return this.PayWay.GetDescription(); } }
        public PayWay PayWay { get; set; }
        public string BankAccount { get; set; }
        public decimal Amount { get; set; }
        public string Extra { get; set; }
        public string CreateAt { get; set; }


    }

    public class FundSourceCreateModel
    {
        public int CapitalAccountID { get; set; }
        public Bank Bank { get; set; }
        public string transferTxNo { get; set; }
        public PayWay payway { get; set; }
        public decimal amount { get; set; }
        public string extra { get; set; }
        public int createBy { get; set; }
    }
    public class FCBankAccount
    {
        public int ID { get; set; }
        public string CNBank { get { return this.Bank.GetDescription(); } }
        public Bank Bank { get; set; }
        public string OwnerName { get; set; } 
        public string BankAccount { get; set; } 
    }
}
