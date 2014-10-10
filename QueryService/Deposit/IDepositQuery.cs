
using DotPay.Common;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.QueryService
{
    public interface IDepositQuery
    {
        #region 人民币虚拟币充值记录
        int CountCNYDepositByUserID(int userID);
        IEnumerable<DepositInListModel> GetCNYDepositByUserID(int userID, int start, int limit);

        int CountVirtualCoinDepositByUserID(int userID, CurrencyType currency);
        IEnumerable<DepositInListModel> GetVirtualCoinDepositByUserID(int userID, CurrencyType currency, int start, int limit);
        #endregion

        IEnumerable<DepositInListModel> GetDepositBySearch(int? userID, string email, int page, int pageCount);
        int CountVirtualCurrencyDepositBySearch(int? userID, DateTime? starttime, DateTime? endtime, CurrencyType currency, DepositState state);
        IEnumerable<VirtualCurrencyDepositInListModel> GetVirtualCurrencyDepositBySearch(int? userID, DateTime? starttime, DateTime? endtime, CurrencyType currency, DepositState state, int page, int pageCount);
        int CountDepositBySearch(int? amount, string email);
        int QueryCompleteCNYDeposit(int depositID, int UserID);

         int CountReceivePayMentTransactionBySearch(CurrencyType currency, DepositState state);
         
    }
}
