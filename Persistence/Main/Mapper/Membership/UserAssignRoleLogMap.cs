using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;
using NHibernate.Type;

namespace DotPay.Persistence
{
    public class UserAssignRoleLogMap : BaseClassMapping<UserAssignRoleLog>
    {
        public UserAssignRoleLogMap()
        {
            Id(u => u.ID, map => { map.Generator(Generators.Identity); });

            Property(x => x.DomainID, map => { map.NotNullable(true); });
            Property(x => x.OperatorID, map => { map.NotNullable(true); });
            Property(x => x.OperateTime, map => map.NotNullable(true));
            Property(x => x.Memo, map => { map.NotNullable(true); });
            Property(x => x.IP, map => { map.NotNullable(true); map.Length(16); });
            Version(x => x.Version, map => { });

            DynamicUpdate(true);
        }
    }
}
