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
    public class RippleInboundToThirdPartyPaymentTxMap : BaseClassMapping<RippleInboundToThirdPartyPaymentTx>
    {
        public RippleInboundToThirdPartyPaymentTxMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));
            Property(x => x.InvoiceID, map => { map.NotNullable(true); map.Unique(true); map.Length(64); });
            Property(x => x.TxID, map => { map.NotNullable(true); });
            Property(x => x.State, map => { map.NotNullable(true); });
            Property(x => x.Destination, map => { map.NotNullable(true); map.Length(34); });
            Property(x => x.RealName, map => { map.NotNullable(true); map.Length(20); });
            Property(x => x.Memo, map => { map.NotNullable(true);  });
            Property(x => x.PayWay, map => { map.NotNullable(true); map.Length(34); });
            Property(x => x.Amount, map => { map.NotNullable(true); map.Precision(12); map.Scale(4); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.DoneAt, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
