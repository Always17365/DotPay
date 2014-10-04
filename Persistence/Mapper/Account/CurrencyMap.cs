using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class CurrencyMap : BaseClassMapping<Currency>
    {
        public CurrencyMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Assigned));

            Property(x => x.Code, map => { map.NotNullable(true); map.Length(6); map.Unique(true); });
            Property(x => x.Name, map => { map.NotNullable(true); map.Length(8); map.Unique(true); });
            Property(x => x.DepositFixedFee, map => { map.NotNullable(true); map.Precision(12); map.Scale(4); });
            Property(x => x.DepositFeeRate, map => { map.NotNullable(true); map.Precision(12); map.Scale(8); });
            Property(x => x.WithdrawFixedFee, map => { map.NotNullable(true); map.Precision(12); map.Scale(4); });
            Property(x => x.WithdrawFeeRate, map => { map.NotNullable(true); map.Precision(12); map.Scale(8); });
            Property(x => x.WithdrawDayLimit, map => { map.NotNullable(true); map.Precision(32); map.Scale(8); });
            Property(x => x.WithdrawOnceMin, map => { map.NotNullable(true); map.Precision(12); map.Scale(8); });
            Property(x => x.WithdrawOnceLimit, map => { map.NotNullable(true); map.Precision(32); map.Scale(8); });
            Property(x => x.WithdrawVerifyLine, map => { map.NotNullable(true); map.Precision(32); map.Scale(8); });
            Property(x => x.NeedConfirm, map => { map.NotNullable(true); });
            Property(x => x.IsEnable, map => { map.NotNullable(true); });
            Property(x => x.IsLocked, map => { map.NotNullable(true); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.CreateBy, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
