using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Common;
using Dotpay.Common.Enum;

namespace Dotpay.Front.ViewModel
{ 
    [Serializable]
    public class AccountBalanceViewModel
    {
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; } 
    }
}
