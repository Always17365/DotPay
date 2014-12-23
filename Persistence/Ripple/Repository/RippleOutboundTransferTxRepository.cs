using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.RippleDomain;
using DotPay.RippleDomain.Repository;
using FC.Framework.NHibernate;
using DotPay.Common;

namespace DotPay.RipplePersistence.Repository
{
    public class RippleOutboundTransferTxRepository : FC.Framework.NHibernate.Repository, IRippleOutboundTransferTxRepository
    {
        public RippleOutboundTransferTxRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public RippleOutboundTransferTx FindByTxId(string txId)
        {
            var result = this._session
                            .QueryOver<RippleOutboundTransferTx>()
                            .Where(d => d.TxId == txId).SingleOrDefault();
            return result;
        }
    }
}
