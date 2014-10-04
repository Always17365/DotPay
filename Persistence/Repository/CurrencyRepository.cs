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
    public class CurrencyRepository : FC.Framework.NHibernate.Repository, ICurrencyRepository
    {
        public CurrencyRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public Currency FindByCurrencyType(CurrencyType currencyType)
        {
            return base.FindById<Currency>((int)(currencyType));
        }
    }
}
