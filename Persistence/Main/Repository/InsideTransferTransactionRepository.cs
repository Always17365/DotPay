using FC.Framework.NHibernate;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.MainDomain;
using DotPay.MainDomain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Persistence.Repository
{
    public class InsideTransferTransactionRepository : FC.Framework.NHibernate.Repository, IInsideTransferTransactionRepository
    {
        public InsideTransferTransactionRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }


        public InsideTransferTransaction FindTransferTxByID(int transferTxID, Common.CurrencyType currency)
        {
            var result = default(InsideTransferTransaction);

            switch (currency)
            {
                case Common.CurrencyType.CNY:
                    result = this._session.QueryOver<CNYInsideTransferTransaction>().Where(tx => tx.ID == transferTxID).SingleOrDefault();
                    break;
                case Common.CurrencyType.USD:
                    result = this._session.QueryOver<USDInsideTransferTransaction>().Where(tx => tx.ID == transferTxID).SingleOrDefault();
                    break;
                default: throw new NotImplementedException();
            }

            return result;
        }


        public InsideTransferTransaction FindTransferTxByID(string transferTxSeq, Common.CurrencyType currency)
        {
            var result = default(InsideTransferTransaction);

            switch (currency)
            {
                case Common.CurrencyType.CNY:
                    result = this._session.QueryOver<CNYInsideTransferTransaction>().Where(tx => tx.SequenceNo == transferTxSeq).SingleOrDefault();
                    break;
                case Common.CurrencyType.USD:
                    result = this._session.QueryOver<USDInsideTransferTransaction>().Where(tx => tx.SequenceNo == transferTxSeq).SingleOrDefault();
                    break;
                default: throw new NotImplementedException();
            }

            return result;
        }
    }
}
