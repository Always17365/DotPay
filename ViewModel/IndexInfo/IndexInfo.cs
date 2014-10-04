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
    public class MarketStatisticsComprehensiveInformation
    {
        public decimal CNY_TwentyFourHoursVolume { get; set; }
        public decimal BTC_TwentyFourHoursVolume { get; set; }
        public decimal LTC_TwentyFourHoursVolume { get; set; }
        public decimal IFC_TwentyFourHoursVolume { get; set; }
        public IEnumerable<MarketGenerateStatistics> AllMarketGenerateStatistics { get; set; }
        public string response { get; set; }
        public int timestamp { get; set; }
        public long totalCount { get; set; }
    }

}
