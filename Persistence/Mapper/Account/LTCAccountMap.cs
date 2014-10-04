using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class LTCAccountMap : BaseClassMapping<LTCAccount>
    {
        public LTCAccountMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.UserID, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.Balance, map => { map.NotNullable(true); map.Precision(16); map.Scale(8); });
            Property(x => x.Locked, map => { map.NotNullable(true); map.Precision(16); map.Scale(8); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.UpdateAt, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
