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
    public class InboundToThirdPartyPaymentTxRepository : FC.Framework.NHibernate.Repository, IInboundToThirdPartyPaymentTxRepository
    {
        public InboundToThirdPartyPaymentTxRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }


        public RippleDomain.RippleInboundToThirdPartyPaymentTx FindByTxId(string txId)
        {
            var result = this._session
                             .QueryOver<RippleInboundToThirdPartyPaymentTx>()
                             .Where(d => d.TxID == txId).SingleOrDefault();

            return result;
        }
    }
}
