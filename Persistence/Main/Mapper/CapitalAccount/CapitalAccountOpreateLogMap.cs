using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;
using NHibernate.Type;

namespace DotPay.Persistence
{
    public class CapitalAccountOpreateLogMap : BaseClassMapping<CapitalAccountOpreateLog>
    {
        public CapitalAccountOpreateLogMap()
        {
            Id(u => u.ID, map => { map.Generator(Generators.Identity); });

            Property(x => x.DomainID, map => { map.NotNullable(true); map.Index("IX_CAPITAL_ACCOUNT_OPREATE_LOG_DOMAINID"); });
            Property(x => x.UniqueID, map => { map.NotNullable(true); map.Length(22); map.Index("IX_CAPITAL_ACCOUNT_OPREATE_LOG_UNIQUEID"); });
            Property(x => x.OperatorID, map => { map.NotNullable(true); map.Index("IX_CAPITAL_ACCOUNT_OPREATER_ID"); });
            Property(x => x.OperateTime, map => { map.NotNullable(true); map.Index("IX_CAPITAL_ACCOUNT_OPREATE_TIME"); });
            Property(x => x.Memo, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
