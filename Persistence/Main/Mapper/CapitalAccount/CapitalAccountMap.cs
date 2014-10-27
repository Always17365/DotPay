using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;
using NHibernate.Type;

namespace DotPay.Persistence
{
    public class CapitalAccountMap : BaseClassMapping<CapitalAccount>
    {
        public CapitalAccountMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.UniqueID, map => { map.NotNullable(true); map.Length(22); map.Index("IX_CAPITAL_ACCOUNT_UNIQUEID"); });
            Property(x => x.Bank, map => { map.NotNullable(true); });
            Property(x => x.OwnerName, map => { map.NotNullable(true); map.Length(30); });
            Property(x => x.BankAccount, map => { map.NotNullable(true); map.Length(20); });
            Property(x => x.IsEnable, map => { map.NotNullable(true); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.CreateBy, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
