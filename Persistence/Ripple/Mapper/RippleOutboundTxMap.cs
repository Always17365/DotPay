using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.RippleDomain;
using NHibernate.Mapping.ByCode;
using DotPay.Common;

namespace DotPay.RipplePersistence.Mapper
{
    public class RippleOutboundTxMap : BaseClassMapping<RippleOutboundTransferTx>
    {
        public RippleOutboundTxMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.TxId, map => { map.NotNullable(true); });
            Property(x => x.Destination, map => { map.NotNullable(true); map.Length(34); });
            Property(x => x.DestinationTag, map => { map.NotNullable(true); });
            Property(x => x.State, map => { map.NotNullable(true); });
            Property(x => x.TargetCurrency, map => { map.NotNullable(true); });
            Property(x => x.TxBlob, map => { map.NotNullable(true); });
            Property(x => x.TargetAmount, map => { map.NotNullable(true); });
            Property(x => x.SourceAmount, map => { map.NotNullable(true); });
            Property(x => x.Fee, map => { map.NotNullable(true); });
            Property(x => x.SourceSendMaxAmount, map => { map.NotNullable(true); map.Precision(18); map.Scale(6); });
            Property(x => x.Reason, map => { map.NotNullable(true); map.Length(34); });
            Version(x => x.Version, map => { });
        }
    }
}
