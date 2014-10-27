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
    public class RippleActiveTxMap : BaseClassMapping<RippleActiveTx>
    {
        public RippleActiveTxMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.TxID, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.TxBlob, map => { map.NotNullable(true); });
            Property(x => x.Source, map => { map.NotNullable(true); map.Length(34); });
            Property(x => x.Destination, map => { map.NotNullable(true); map.Length(34); });
            Property(x => x.State, map => { map.NotNullable(true); });
            Property(x => x.Amount, map => { map.NotNullable(true); map.Precision(10); map.Scale(6); });
            Property(x => x.Fee, map => { map.NotNullable(true); map.Precision(8); map.Scale(6); });
            Property(x => x.Memo, map => { map.NotNullable(true); map.Length(255); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.LastUpdateAt, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
