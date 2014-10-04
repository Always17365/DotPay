using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class BTCPaymentAddressMap : BaseClassMapping<BTCPaymentAddress>
    {
        public BTCPaymentAddressMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.UserID, map => { map.NotNullable(true); });
            Property(x => x.AccountID, map => { map.NotNullable(true); map.Unique(true); });
            Property(x => x.Address, map => { map.NotNullable(true); map.Length(34); map.Unique(true); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
