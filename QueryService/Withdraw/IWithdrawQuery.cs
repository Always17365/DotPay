using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotPay.Common;

namespace DotPay.QueryService
{
    public interface IWithdrawQuery
    {
        int CountWithdrawBySearch(int? amount, string transferId, WithdrawState state);
        IEnumerable<WithdrawListModel> GetWithdrawBySearch(int? amount, string transferId, WithdrawState state, int page, int pageCount);

        #region 提现查询
        int CountMyCoinWithdraw_Sql(int userID, CurrencyType currency);
        IEnumerable<WithdrawListModel> GetMyCNYWithdraw(int userID, int start, int limit);
        IEnumerable<WithdrawListModel> GetMyVirtualCoinWithdraw(int userID, CurrencyType currency, int start, int limit);
        #endregion

        IEnumerable<Bankoutlets> GetBank(int cityid);

        int CountVirtualCoinWithdrawBySearch(int? userID, CurrencyType currencyType, VirtualCoinTxState state);
        IEnumerable<VirtualCoinWithdrawListModel> GetVirtualCoinWithdrawBySearch(int? userID, CurrencyType currencyType, VirtualCoinTxState state, int page, int pageCount);
       
        
    }
}
