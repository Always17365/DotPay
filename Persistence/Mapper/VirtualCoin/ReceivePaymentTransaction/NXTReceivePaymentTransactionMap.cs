using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class NXTReceivePaymentTransactionMap : BaseClassMapping<NXTReceivePaymentTransaction>
    {
        public NXTReceivePaymentTransactionMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.TxID, map => { map.NotNullable(true); map.Unique(true); map.Length(67); });
            Property(x => x.State, map => { map.NotNullable(true); });
            Property(x => x.Amount, map => { map.NotNullable(true); map.Precision(14); map.Scale(4); });
            Property(x => x.Address, map => { map.NotNullable(true); map.Length(34); });
            Property(x => x.Confirmation, map => { map.NotNullable(true); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.ConfirmAt, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
