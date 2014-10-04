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
    public class MarketListModel
    {
        public int ID { get; set; }
        public string Name
        {
            get
            {
                var market = new MarketType();
                market.TargetCurrency = this.TargetCurrency;
                market.BaseCurrency = this.BaseCurrency;
                return market.ToString();
            }
        }
        public CurrencyType TargetCurrency { get; set; }
        public CurrencyType BaseCurrency { get; set; }
        public decimal FeeRate { get; set; }
        public int PriceDecimal { get; set; }
        public int AmountDecimal { get; set; }
        public int VolumeDecimal { get; set; }
        public decimal MinAmount { get; set; }
        public bool IsOpen { get; set; }
        public int UpdateAt { get; set; }
        public int RuningCount { get; set; }
    }

}
