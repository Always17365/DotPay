
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.QueryService
{
    public interface ICapitalAccountQuery
    {
        int CountCapitalAccountBySearch(string name);
        IEnumerable<CapitalAccountListModel> GetCapitalAccountBySearch(string name, int page, int pageCount);
    }
}
