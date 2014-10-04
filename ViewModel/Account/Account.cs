using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DotPay.Common;

namespace DotPay.ViewModel
{
    public class AccountModel
    {
        public int ID { get; set; }
        public int CurrencyID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal Locked { get; set; }
        public decimal Sum { get; set; }
        public decimal EstimateAmount { get; set; }
    }
}
