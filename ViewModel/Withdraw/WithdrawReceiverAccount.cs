using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DotPay.Common;

namespace DotPay.ViewModel
{
    public class WithdrawReceiverAccountModel
    {
        public int ID { get; set; }
        public string BankAccount { get; set; } 
        public Bank Bank { get; set; }
        //public int ProvinceID { get; set; }
        //public int CityID { get; set; }
        //public int BankOutletsID { get; set; }
    }
}
