using FC.Framework.NHibernate;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.Domain;
using DotPay.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Persistence.Repository
{
    public class WithdrawRepository : FC.Framework.NHibernate.Repository, IWithdrawRepository
    {
        public WithdrawRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public IWithdraw FindByIdAndCurrency(int id, Common.CurrencyType currency)
        {
            var withdraw = default(IWithdraw);

            switch (currency)
            {
                case CurrencyType.CNY:
                    withdraw = this._session.QueryOver<CNYWithdraw>().Where(wd => wd.ID == id).SingleOrDefault();
                    break;
              
                default:
                    throw new NotImplementedException();
            }

            return withdraw;
        }


        public IWithdraw FindByUniqueIdAndCurrency(string uniqueID, CurrencyType currency)
        {
            var withdraw = default(IWithdraw);

            switch (currency)
            {
                case CurrencyType.CNY:
                    withdraw = this._session.QueryOver<CNYWithdraw>().Where(wd => wd.UniqueID == uniqueID).SingleOrDefault();
                    break;
               
                default:
                    throw new NotImplementedException();
            }

            return withdraw;
        }
    }
}
