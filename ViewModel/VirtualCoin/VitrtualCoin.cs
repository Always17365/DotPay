using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DotPay.Common;

namespace DotPay.ViewModel
{
    public class VitrtualCoinBalanceStatistics
    {
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public int LastUpdateAt { get; set; }
    }
    public class VirtualCoinStatis
    {
        public decimal Amount { get; set; }
        public decimal AmountDeposit { get; set; }
        public decimal AmountWithdraw { get; set; }
        public int Datetime { get; set; }
        public decimal Volume { get; set; }
    }
}
