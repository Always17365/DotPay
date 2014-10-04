using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;
using NHibernate.Type;

namespace DotPay.Persistence
{
    public class LTCWithdrawProcessLogMap : BaseClassMapping<LTCWithdrawProcessLog>
    {
        public LTCWithdrawProcessLogMap()
        {
            Id(u => u.ID, map => { map.Generator(Generators.Identity); });

            Property(x => x.DomainID, map => { map.NotNullable(true); map.Index("IX_LTC_WITHDRAW_PROCESS_LOG_DOMAIN_ID"); });
            Property(x => x.OperatorID, map => { map.NotNullable(true); map.Index("IX_LTC_WITHDRAW_PROCESS_LOG_OPERATEOR_ID"); });
            Property(x => x.OperateTime, map => { map.NotNullable(true); map.Index("IX_LTC_WITHDRAW_PROCESS_LOG_OPERATE_TIME"); });
            Property(x => x.Memo, map => { map.NotNullable(true); });
            Property(x => x.IP, map => { map.NotNullable(true); map.Length(16); });
            Version(x => x.Version, map => { });
        }
    }
}
