
using DotPay.Common;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.QueryService
{
    public interface ICurrencyQuery
    {
        int GetCurrencyCountBySearch(string code, string name);
        /// <summary>
        ///  判断币种是否已经存在了
        ///  <remarks>result:Item1 is code exist, Item2 is name exist</remarks>
        /// </summary>
        /// <param name="code">currency code</param>
        /// <param name="name">currency name</param>
        /// <returns></returns>
        Tuple<bool, bool> ExistCurrency(string code, string name);
        IEnumerable<CurrencyListModel> GetCurrenciesBySearch(string code, string name, int page, int pageCount);
        IEnumerable<CurrencyListModel> GetAllCurrencies();
        CurrencyListModel GetCurrency(CurrencyType currency);
    }
}
