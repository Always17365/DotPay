using System.Linq;
using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class PreRegistrationMap : BaseClassMapping<PreRegistration>
    {
        public PreRegistrationMap()
        {
            Id(u => u.ID, map => { map.Generator(Generators.Identity); });

            Property(x => x.Email, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.EmailValidateToken, map => { map.NotNullable(true); map.Length(32); });
            Property(x => x.IsEmailVerify, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });

            DynamicUpdate(true);
        }

    }
}
