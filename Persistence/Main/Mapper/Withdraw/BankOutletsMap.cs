using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class BankOutletsMap : BaseClassMapping<BankOutlets>
    {
        public BankOutletsMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.Bank, map => { map.NotNullable(true); });
            Property(x => x.ProvinceID, map => { map.NotNullable(true); });
            Property(x => x.CityID, map => { map.NotNullable(true); });
            Property(x => x.Name, map => { map.NotNullable(true); });
            Property(x => x.IsDelete, map => { map.NotNullable(true); });
            Property(x => x.DeleteBy, map => { map.NotNullable(true); });

            Version(x => x.Version, map => { });
        }
    }
}
