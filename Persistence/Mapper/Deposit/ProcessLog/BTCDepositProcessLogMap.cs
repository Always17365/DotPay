using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;
using NHibernate.Type;

namespace DotPay.Persistence
{
    public class BTCDepositProcessLogMap : BaseClassMapping<BTCDepositProcessLog>
    {
        public BTCDepositProcessLogMap()
        {
            Id(u => u.ID, map => { map.Generator(Generators.Identity); });

            Property(x => x.DomainID, map => { map.NotNullable(true); map.Index("IX_BTC_DEPOSIT_PROCESSLOG_DOMAINID"); });
            Property(x => x.UniqueID, map => { map.NotNullable(true); map.Length(22); map.Index("IX_BTC_DEPOSIT_PROCESSLOG_UNIQUEID"); });
            Property(x => x.OperatorID, map => { map.NotNullable(true); map.Index("IX_BTC_DEPOSIT_PROCESSLOG_OPERATOR_ID"); });
            Property(x => x.OperateTime, map => { map.NotNullable(true); map.Index("IX_BTC_DEPOSIT_PROCESSLOG_OPERATOR_TIME"); });
            Property(x => x.Memo, map => { map.NotNullable(true); });
            Property(x => x.IP, map => { map.NotNullable(true); map.Length(16); });
            Version(x => x.Version, map => { });
        }
    }
}
