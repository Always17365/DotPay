using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class ToTenpayTransferTransactionMap : BaseClassMapping<ToTenpayTransferTransaction>
    {
        public ToTenpayTransferTransactionMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.SequenceNo, map => { map.NotNullable(true); map.Unique(true); map.Length(40); });
            Property(x => x.TxId, map => { map.NotNullable(true); map.Length(65); });
            Property(x => x.PayWay, map => { map.NotNullable(true); });
            Property(x => x.SourcePayWay, map => { map.NotNullable(true); });
            Property(x => x.Account, map => { map.NotNullable(true); });
            Property(x => x.Amount, map => { map.NotNullable(true); map.Precision(10); map.Scale(2); });
            Property(x => x.TransferNo, map => { map.NotNullable(true); map.Length(30); });
            Property(x => x.State, map => { map.NotNullable(true); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.DoneAt, map => { map.NotNullable(true); });
            Property(x => x.Memo, map => { map.NotNullable(true); }); 
            Version(x => x.Version, map => { });
        }
    }
}
