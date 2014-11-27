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
    public class RippleInboundTxMap : BaseClassMapping<RippleInboundTx>
    {
        public RippleInboundTxMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.TxID, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.DestinationTag, map => { map.NotNullable(true); });
            Property(x => x.Amount, map => { map.NotNullable(true); map.Length(34); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); map.Length(34); }); 
            Version(x => x.Version, map => { });
        }
    }
}
