using FC.Framework.Repository;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Repository
{
    public interface IAccountVersionRepository : IRepository
    {
        AccountVersion FindByDepositIDAndCurrency(int depositID, CurrencyType currency); 
        AccountVersion FindByWithdrawIDAndCurrency(string withdrawUniqueID, CurrencyType currency);

        void Remove(AccountVersion accountVersion);
    }
}
