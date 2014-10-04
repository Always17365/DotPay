using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    public struct MarketType : IEquatable<MarketType>
    {
        #region CNY Market
        public static MarketType CNY_BTC
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.CNY;
                market.TargetCurrency = CurrencyType.BTC;
                return market;
            }
        }
        public static MarketType CNY_LTC
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.CNY;
                market.TargetCurrency = CurrencyType.LTC;
                return market;
            }
        }
        public static MarketType CNY_IFC
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.CNY;
                market.TargetCurrency = CurrencyType.IFC;
                return market;
            }
        }
        public static MarketType CNY_DOGE
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.CNY;
                market.TargetCurrency = CurrencyType.DOGE;
                return market;
            }
        }
        public static MarketType CNY_NXT
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.CNY;
                market.TargetCurrency = CurrencyType.NXT;
                return market;
            }
        } 
        public static MarketType CNY_STR
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.CNY;
                market.TargetCurrency = CurrencyType.STR;
                return market;
            }
        }
        public static MarketType CNY_FBC
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.CNY;
                market.TargetCurrency = CurrencyType.FBC;
                return market;
            }
        } 
	#endregion

        #region BTC Market
        public static MarketType BTC_NXT
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.BTC;
                market.TargetCurrency = CurrencyType.NXT;
                return market;
            }
        }
        public static MarketType BTC_LTC
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.BTC;
                market.TargetCurrency = CurrencyType.LTC;
                return market;
            }
        }
        public static MarketType BTC_IFC
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.BTC;
                market.TargetCurrency = CurrencyType.IFC;
                return market;
            }
        }

        public static MarketType BTC_DOGE
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.BTC;
                market.TargetCurrency = CurrencyType.DOGE;
                return market;
            }
        }
        
        public static MarketType BTC_STR
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.BTC;
                market.TargetCurrency = CurrencyType.STR;
                return market;
            }
        }
        #endregion

        #region LTC Market
        public static MarketType LTC_NXT
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.LTC;
                market.TargetCurrency = CurrencyType.NXT;
                return market;
            }
        } 
        public static MarketType LTC_IFC
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.LTC;
                market.TargetCurrency = CurrencyType.IFC;
                return market;
            }
        }

        public static MarketType LTC_DOGE
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.LTC;
                market.TargetCurrency = CurrencyType.DOGE;
                return market;
            }
        }
        #endregion

        #region IFC Market
        public static MarketType IFC_NXT
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.IFC;
                market.TargetCurrency = CurrencyType.NXT;
                return market;
            }
        }
        public static MarketType IFC_STR
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.IFC;
                market.TargetCurrency = CurrencyType.STR;
                return market;
            }
        }

        public static MarketType IFC_DOGE
        {
            get
            {
                var market = new MarketType();
                market.BaseCurrency = CurrencyType.IFC;
                market.TargetCurrency = CurrencyType.DOGE;
                return market;
            }
        }
        #endregion

        public CurrencyType BaseCurrency { get; set; }
        public CurrencyType TargetCurrency { get; set; }

        public static implicit operator string(MarketType market)
        {
            return market.BaseCurrency.ToString() + "_" + market.TargetCurrency.ToString();
        }

        public override string ToString()
        {
            return this.BaseCurrency.ToString() + "_" + this.TargetCurrency.ToString();
        }


        public bool Equals(MarketType other)
        {
            return this.BaseCurrency == other.BaseCurrency && this.TargetCurrency == other.TargetCurrency;
        }
    }
}
