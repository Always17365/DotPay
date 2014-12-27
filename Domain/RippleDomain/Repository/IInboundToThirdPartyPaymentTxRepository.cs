using DotPay.Common;
using FC.Framework.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.RippleDomain.Repository
{
    public interface IInboundToThirdPartyPaymentTxRepository : IRepository
    {
        RippleInboundToThirdPartyPaymentTx FindByTxId(string txId);
    }
}
