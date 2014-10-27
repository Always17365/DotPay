using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;
using NHibernate.Type;

namespace DotPay.Persistence
{
    public class VipSettingLogMap : BaseClassMapping<VipSettingLog>
    {
        public VipSettingLogMap()
        {
            Id(u => u.ID, map => { map.Generator(Generators.Identity); });

            Property(x => x.DomainID, map => { map.NotNullable(true); });
            Property(x => x.OperatorID, map => { map.NotNullable(true); });
            Property(x => x.OperateTime, map => map.NotNullable(true));
            Property(x => x.Memo, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { }); 
        }
    }
}
