using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class SmsInterfaceMap : BaseClassMapping<SmsInterface>
    {
        public SmsInterfaceMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.SmsType, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.Account, map => { map.NotNullable(true); });
            Property(x => x.Password, map => { map.NotNullable(true); });
            Property(x => x.CreateBy, map => { map.NotNullable(true); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
