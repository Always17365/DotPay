using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotPay.Common;

namespace DotPay.QueryService
{
    public interface IBankOutletsQuery
    {
        IEnumerable<WithdrawBankOutletsModel> GetBankoutletsByProvinceIDAndCityID(Bank bank, int provinceID, int cityID);
        int GetBankInfoCountBySearch(int? bankType, int? province, int? city);
        IEnumerable<Bankoutlets> GetBankInfoBySearch(int? bankType, int? province, int? city, int page, int pageCount);
    }
}
