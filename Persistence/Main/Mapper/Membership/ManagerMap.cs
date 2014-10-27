using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class ManagerMap : BaseClassMapping<Manager>
    {
        public ManagerMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.UserID, map => { map.NotNullable(true); map.UniqueKey("IX_MANAGER_USER_ROLE"); });
            Property(x => x.Type, map => { map.NotNullable(true); map.UniqueKey("IX_MANAGER_USER_ROLE"); });
            Property(x => x.CreateAt, map => map.NotNullable(true));
            Version(x => x.Version, map => { });

            DynamicUpdate(true);
        }
    }
}
