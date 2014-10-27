using FC.Framework.Repository;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Repository
{
    public interface IWithdrawRepository : IRepository
    {
        IWithdraw FindByIdAndCurrency(int id, CurrencyType currency);
        IWithdraw FindByUniqueIdAndCurrency(string uniqueID, CurrencyType currency);
    }
}
