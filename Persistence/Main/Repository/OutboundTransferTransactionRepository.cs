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
    public class OutboundTransferTransactionRepository : FC.Framework.NHibernate.Repository, IOutboundTransferTxRepository
    {
        public OutboundTransferTransactionRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public OutboundTransferTransaction FindTransferTxBySeq(string sequenceNo)
        {
            return this._session.QueryOver<OutboundTransferTransaction>().Where(t => t.SequenceNo == sequenceNo).SingleOrDefault();
        }
    }
}
