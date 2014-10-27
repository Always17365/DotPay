using FC.Framework.Repository;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Repository
{
    public interface IAccountRepository : IRepository
    {
        Account FindByUserIDAndCurrency(int userID, CurrencyType currency);
        Account FindByIDAndCurrency(int accountID, CurrencyType currency);
    }
}
