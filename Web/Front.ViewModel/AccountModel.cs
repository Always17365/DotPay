using System;
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
