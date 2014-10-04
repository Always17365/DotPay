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
    public class DepositInListModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Email { get; set; }
        public int FundSourceID { get; set; }
        public int FundExtra { get; set; }
        public string TxID { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string CNState { get { return LangHelpers.Lang(this.State.GetDescription()); } }
        public string CNPayWay { get { return  LangHelpers.Lang(this.PayWay.GetDescription()); } }
        public PayWay PayWay { get; set; }
        public DepositState State { get; set; }
        public int CreateAt { get; set; }
        public string Memo { get; set; }
    }
    public class VirtualCurrencyDepositInListModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int AccountID { get; set; }
        public decimal Amount { get; set; }
        public string TxID { get; set; }
        public string Address { get; set; }
        public string UniqueID { get; set; }
        public string CNState { get { return  LangHelpers.Lang(this.State.GetDescription()); } }
        public DepositState State { get; set; }
        public int CreateAt { get; set; }
        public string Memo { get; set; }
    }

}
