using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class OpenAuthShipMap : BaseClassMapping<OpenAuthShip>
    {
        public OpenAuthShipMap()
        {
            Id(u => u.ID, map => { map.Generator(Generators.Identity); });

            Property(x => x.UserID, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.OpenID, map => { map.NotNullable(true); map.UniqueKey("IX_OPEN_AUTH_ID_TYPE"); });
            Property(x => x.Type, map => { map.NotNullable(true); map.UniqueKey("IX_OPEN_AUTH_ID_TYPE"); });
            Property(x => x.CreateAt, map => map.NotNullable(true));
            Version(x => x.Version, map => { });

            DynamicUpdate(true);
        }
    }
}
