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
    public class CapitalAccountListModel
    {
        public int ID { get; set; }
        public string CNBank { get { return this.Bank.GetDescription(); } }
        public Bank Bank { get; set; }
        public string OwnerName { get; set; }
        public string BankAccount { get; set; }
        public bool IsEnable { get; set; }
        public string CreateAt { get; set; }
        public string CreateBy { get; set; }
    }

    public class CapitalAccountCreateModel
    {
        public Bank Bank { get; set; }
        public string OwnerName { get; set; }
        public string BankAccount { get; set; }
    }
}
