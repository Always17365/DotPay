using FC.Framework.NHibernate;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain;
using DotPay.MainDomain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Persistence.Repository
{
    public class AccountRepository : FC.Framework.NHibernate.Repository, IAccountRepository
    {
        public AccountRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public Account FindByUserIDAndCurrency(int userID, CurrencyType currency)
        {
            var account = default(Account);

            switch (currency)
            {
                case CurrencyType.CNY:
                    account = this._session.QueryOver<CNYAccount>().Where(act => act.UserID == userID).SingleOrDefault();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return account;
        }


        public Account FindByIDAndCurrency(int accountID, CurrencyType currency)
        {
            var account = default(Account);

            switch (currency)
            {
                case CurrencyType.CNY:
                    account = this._session.QueryOver<CNYAccount>().Where(act => act.ID == accountID).SingleOrDefault();
                    break;

                default:
                    throw new NotImplementedException();
            }

            return account;
        }
    }
}
