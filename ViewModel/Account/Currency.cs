using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DotPay.ViewModel
{
    public class CurrencyListModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal DepositFixedFee { get; set; }
        public decimal DepositFeeRate { get; set; }
        public decimal WithdrawFeeRate { get; set; }
        public decimal WithdrawFixedFee { get; set; }
        public decimal WithdrawOnceLimit { get; set; }
        public decimal WithdrawOnceMin { get; set; }
        public decimal WithdrawDayLimit { get; set; }
        public decimal WithdrawVerifyLine { get; set; }
        public decimal NeedConfirm { get; set; }
        public bool IsEnable { get; set; }
        public bool IsLocked { get; set; }
        public int CreateAt { get; set; }
    }

    public class CurrencyCreateModel
    {
        public int CurrencyID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        //public decimal DepositFixedFee { get; set; }
        //public decimal DepositFeeRate { get; set; }
        //public decimal WithdrawFeeRate { get; set; }
        //public decimal WithdrawFixedFee { get; set; }
        //public int WithdrawOnceLimit { get; set; }
        //public int WithdrawDayLimit { get; set; }
        //public int WithdrawVerifyLine { get; set; }
        //public int NeedConfirm { get; set; }
    }
}
