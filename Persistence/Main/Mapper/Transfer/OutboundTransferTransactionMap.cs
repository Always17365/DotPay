using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class OutboundTransferTransactionMap : BaseClassMapping<OutboundTransferTransaction>
    {
        public OutboundTransferTransactionMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.SequenceNo, map => { map.NotNullable(true); map.Unique(true); map.Length(40); });
            Property(x => x.FromUserID, map => { map.NotNullable(true); });
            Property(x => x.SourceAmount, map => { map.NotNullable(true); map.Precision(10); map.Scale(2); });
            Property(x => x.TargetAmount, map => { map.NotNullable(true); map.Precision(14); map.Scale(2); });
            Property(x => x.TargetCurrency, map => { map.NotNullable(true); });
            Property(x => x.PayWay, map => { map.NotNullable(true); });
            Property(x => x.TransferNo, map => { map.NotNullable(true); });
            Property(x => x.Description, map => { map.NotNullable(true); map.Length(200); });
            Property(x => x.Destination, map => { map.NotNullable(true); });
            Property(x => x.State, map => { map.NotNullable(true); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.DoneAt, map => { map.NotNullable(true); });
            Property(x => x.Memo, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
