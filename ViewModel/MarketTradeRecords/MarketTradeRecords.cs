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
    public class MarketTradeRecordsListModel
    {
        public int ID { get; set; }
        public int UniqueID { get; set; }
        public int AskUserID { get; set; }
        public int BidUserID { get; set; }
        public string CNTrend { get { return this.Trend.GetDescription(); } }
        public TradeTrend Trend { get; set; }
        public string MarketType { get; set; }
        public int AskID { get; set; }
        public int BidID { get; set; }
        public string CNType { get { return this.Type.GetDescription(); } }
        public string Market { get; set; }
        public OrderType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Volume { get; set; }
        public decimal Price { get; set; }
        public decimal AskFee { get; set; }
        public decimal BidFee { get; set; }
        public int CreateAt { get; set; }

    }

    public class MyAllTheTradeHistoryParcel
    {
        public int Num { get; set; }
        public IEnumerable<TradeListFrontModel> MyAllTheTradeHistory { get; set; }
    }
    public class TradeListFrontModel
    {
        public string MarketType { get; set; }
        public string CNType { get { return this.Type.GetDescription(); } }
        public OrderType Type { get; set; }
        public string Amount { get; set; }
        public string Volume { get; set; }
        public string VolumePercent { get; set; }
        public string Price { get; set; }
        public int CreateAt { get; set; }

    }
    #region 边侧栏需求
    public class TradingMarketInformation
    {
        public IEnumerable<SidebaBaseMarketInformation> sidebarBaseMarketInformation { get; set; } 
    }
    public class SidebaBaseMarketInformation
    {       
        public IEnumerable<MarketsInformation> MarketsInformation { get; set; }        
        public decimal TwentyFourHoursVolume { get; set; }
        public string CurrencyType { get; set; }
    }
    public class MarketsInformation
    {
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public string MarketType { get; set; }
        public string Price { get; set; }
        public string Change { get; set; }
        public string BaseCurrencyVolume { get; set; }
    }
    #endregion
}
