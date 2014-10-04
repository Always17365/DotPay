using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using DotPay.ViewModel;
using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;

namespace DotPay.QueryService.Impl
{
    public class VirtualCoinStatisticsQuery : AbstractQuery, IVirtualCoinStatisticsQuery
    {
        public IEnumerable<VirtualCoinStatis> GetWalletByVirtualCoin(CurrencyType currencyType)
        {
            var result = this.Context.Sql(getWalletByVirtualCoin_Sql.FormatWith(currencyType.ToString().ToUpper()))
                                   .QueryMany<VirtualCoinStatis>();

            return result;
        }
        public IEnumerable<VirtualCoinStatis> GetRateOfFlowByVirtualCoin(CurrencyType currencyType)
        {
            var paramters = new object[] { DateTime.Now.ToUnixTimestamp() };
            var result = this.Context.Sql(getRateOfFlowByVirtualCoin_Sql.FormatWith(currencyType.ToString()))
                                   .Parameters(paramters)
                                   .QueryMany<VirtualCoinStatis>();

            return result;
        }

        public IEnumerable<VirtualCoinStatis> OrderStatistics(CurrencyType currencyType)
        {
            var result_btc = new List<VirtualCoinStatis> { new VirtualCoinStatis { } }; 
            if (currencyType != CurrencyType.BTC)
            {
                result_btc = this.Context.Sql(btcOrderStatistics_Sql.FormatWith(currencyType.ToString()))
                                   .QueryMany<VirtualCoinStatis>();
            }
            var result_ltc = new List<VirtualCoinStatis> { new VirtualCoinStatis { } };
            if (currencyType == CurrencyType.DOGE || currencyType == CurrencyType.IFC || currencyType == CurrencyType.NXT)
            {
                result_ltc = this.Context.Sql(ltcOrderStatistics_Sql.FormatWith(currencyType.ToString()))
                                     .QueryMany<VirtualCoinStatis>();
            }
            var result_cny = this.Context.Sql(cnyOrderStatistics_Sql.FormatWith(currencyType.ToString()))
                                  .QueryMany<VirtualCoinStatis>();

            var result = (result_btc.Union<VirtualCoinStatis>(result_ltc)).Union<VirtualCoinStatis>(result_cny);
            return result;
        }
        public IEnumerable<VirtualCoinStatis> TransactionStatistics(CurrencyType currencyType)
        {
            var result_btc = new List<VirtualCoinStatis> { new VirtualCoinStatis { } };
            if (currencyType != CurrencyType.BTC)
            {
                result_btc = this.Context.Sql(btcTransactionStatistics_Sql.FormatWith(currencyType.ToString()))
                                     .QueryMany<VirtualCoinStatis>();
            }
            var result_ltc = new List<VirtualCoinStatis> { new VirtualCoinStatis { } };
            if (currencyType == CurrencyType.DOGE || currencyType == CurrencyType.IFC || currencyType == CurrencyType.NXT) 
            {
                result_ltc = this.Context.Sql(ltcTransactionStatistics_Sql.FormatWith(currencyType.ToString()))
                                     .QueryMany<VirtualCoinStatis>();
            }
            var result_cny = this.Context.Sql(cnyTransactionStatistics_Sql.FormatWith(currencyType.ToString()))
                                   .QueryMany<VirtualCoinStatis>();

            var result = (result_btc.Union<VirtualCoinStatis>(result_ltc)).Union<VirtualCoinStatis>(result_cny);
            return result;


        }
        public IEnumerable<VitrtualCoinBalanceStatistics> GetVitrtualCoinBalanceStatistics()
        {
            return this.Context.Sql(balance_sql).QueryMany<VitrtualCoinBalanceStatistics>();
        }

        #region SQL
        private readonly string balance_sql = "SELECT Currency,Amount,LastUpdateAt FROM " + Config.Table_Prefix + @"VirtualCoinStatistics";

        /********************************************************************************************/
        private readonly string btcTransactionStatistics_Sql =
                       @"SELECT   SUM(Volume) AS Volume,  SUM(Volume * Price) AS Amount 
                                    FROM   " + Config.Table_Prefix + @"trade_btc_{0}";

        private readonly string ltcTransactionStatistics_Sql =
                       @"SELECT   SUM(Volume) AS Volume,  SUM(Volume * Price) AS Amount 
                                    FROM   " + Config.Table_Prefix + @"trade_ltc_{0}";

        private readonly string cnyTransactionStatistics_Sql =
                       @"SELECT   SUM(Volume) AS Volume,  SUM(Volume * Price)  AS Amount 
                                    FROM   " + Config.Table_Prefix + @"trade_cny_{0}";

        /********************************************************************************************/

        private readonly string btcOrderStatistics_Sql =
                       @"SELECT   SUM(Volume) AS Volume,  SUM(Volume * Price) AS Amount 
                                    FROM   " + Config.Table_Prefix + @"order_btc_{0}                                    
                                   WHERE    State = 2 and  type =1";

        private readonly string ltcOrderStatistics_Sql =
                       @"SELECT   SUM(Volume) AS Volume,  SUM(Volume * Price) AS Amount 
                                    FROM   " + Config.Table_Prefix + @"order_ltc_{0}                                    
                                   WHERE    State = 2 and  type =1";
                               
        private readonly string cnyOrderStatistics_Sql = 
                       @"SELECT   SUM(Volume) AS Volume,  SUM(Volume * Price) AS Amount 
                                    FROM   " + Config.Table_Prefix + @"order_cny_{0}                                 
                                   WHERE    State = 2 and  type =2";
        
        /********************************************************************************************/
        private readonly string getWalletByVirtualCoin_Sql =
                               @"SELECT    SUM(Amount) AS Amount,MIN(LastUpdateAt) AS `Datetime`
                                    FROM   " + Config.Table_Prefix + @"virtualcoinstatistics 
                                   WHERE   StatisticsFlg like '{0}_%'";

        private readonly string getRateOfFlowByVirtualCoin_Sql =
                              @"(SELECT
                                    (SELECT    SUM(Amount) 
                                       FROM    " + Config.Table_Prefix + @"{0}deposit 
                                      WHERE    State = 3 and  type =2 
                                        AND    CreateAt >= @0) AS AmountDeposit 
                                          ,
                                    (SELECT    SUM(Amount + TxFee) 
                                       FROM    " + Config.Table_Prefix + @"{0}withdraw
                                      WHERE    State = 4 
                                        AND    CreateAt >= @0) AS AmountWithdraw
                                )
                                UNION     ALL
                                (SELECT
                                    (SELECT    SUM(Amount) 
                                       FROM    " + Config.Table_Prefix + @"{0}deposit 
                                      WHERE    State = 3 and  type =2
                                    )          AS AmountDeposit 
                                          ,
                                    (SELECT    SUM(Amount + TxFee) 
                                       FROM    " + Config.Table_Prefix + @"{0}withdraw
                                      WHERE    State = 4 
                                     )         AS AmountWithdraw
                                )";


        #endregion
    }
}
