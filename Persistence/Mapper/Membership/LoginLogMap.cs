using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;
using NHibernate.Type;

namespace DotPay.Persistence
{
    public class LoginLogMap : BaseClassMapping<LoginLog>
    {
        public LoginLogMap()
        {
            Id(u => u.ID, map => { map.Generator(Generators.Identity); });

            Property(x => x.UserID, map => { map.NotNullable(true); });
            Property(x => x.LoginTime, map => { map.NotNullable(true); });
            Property(x => x.IP, map => { map.NotNullable(true); map.Length(16); });
            Version(x => x.Version, map => { });

            DynamicUpdate(true);
        }
    }
}
