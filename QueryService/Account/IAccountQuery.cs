
using DotPay.Common;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.QueryService
{
    public interface IAccountQuery
    {
        IEnumerable<AccountModel> GetAccountsByUserID(int userID);
        AccountModel GetAccountByUserID(int userID,CurrencyType currency);
    }
}
