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
    public class DepositRepository : FC.Framework.NHibernate.Repository, IDepositRepository
    {
        public DepositRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public VirtualCoinDeposit FindByTxIDAndCurrency(string txid, CurrencyType currency)
        {
            var deposit = default(VirtualCoinDeposit);

            switch (currency)
            {
             
                default:
                    throw new NotImplementedException();
            }

            return deposit;
        }


        public VirtualCoinDeposit FindByReceivePaymentTxUniqueIDAndCurrency(string txUnqiueId, CurrencyType currency)
        {
            var deposit = default(VirtualCoinDeposit);

            switch (currency)
            { 
                //case CurrencyType.STR:
                //    deposit = this._session.QueryOver<STRDeposit>().Where(d => d.ReceivePaymentTxUniqueID == txUnqiueId).SingleOrDefault();
                //    break;
                default:
                    throw new NotImplementedException();
            }

            return deposit;
        }
    }
}
