using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using FC.Framework;

namespace FullCoin.ViewModel
{
    public class VirtualCoinStatis
    {
        public decimal Amount { get; set; }
        public decimal AmountDeposit { get; set; }
        public decimal AmountWithdraw { get; set; }
        public int Datetime { get; set; }
        public int Volume { get; set; }
    }
}
