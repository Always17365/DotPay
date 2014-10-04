using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotPay.Common;

namespace DotPay.QueryService
{
    public interface IVirtualCoinStatisticsQuery
    {
        IEnumerable<VirtualCoinStatis> GetWalletByVirtualCoin(CurrencyType currencyType);
        IEnumerable<VirtualCoinStatis> GetRateOfFlowByVirtualCoin(CurrencyType currencyType);
        IEnumerable<VirtualCoinStatis> TransactionStatistics(CurrencyType currencyType);
        IEnumerable<VirtualCoinStatis> OrderStatistics(CurrencyType currencyType);
        IEnumerable<VitrtualCoinBalanceStatistics> GetVitrtualCoinBalanceStatistics();
        
    }
}
