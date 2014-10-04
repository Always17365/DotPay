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
    public class StatisticsListModel
    {
        public long Date { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal BaseVolume { get; set; }
        public decimal TargetVolume { get; set; }

    }

    public class MarketGenerateStatistics
    {
        public MarketType MarketType { get; set; }
        public string BaseCurrency { get { return this.MarketType.BaseCurrency.ToString(); } }
        public string TargetCurrency { get { return this.MarketType.TargetCurrency.ToString(); } }
        public decimal NewestPrice { get; set; }
        public decimal TwentyFourHoursTheRiseAndDecline { get; set; }
        public decimal TwentyFourHoursLowPrice { get; set; }
        public decimal TwentyFourHoursHighPrice { get; set; }
        public string TwentyFourHoursVolume { get; set; }
        public string TOPBID  { get; set; }//买单
        public string TOPASK  { get; set; } //卖单
    }
    public class CurrentStatisticsModel
    {
        public decimal Increase { get { return this.MarketOpenPrice == 0 ? 0 : ((this.NewestPrice - this.MarketOpenPrice) / this.MarketOpenPrice); } }
        public decimal MarketOpenPrice { get; set; }
        public decimal NewestPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal FirstAskPrice { get; set; }
        public decimal FirstBidPrice { get; set; }
        public decimal MarketLowPrice { get; set; }
        public decimal BaseCurrencyVolume { get; set; }

    } 
}
