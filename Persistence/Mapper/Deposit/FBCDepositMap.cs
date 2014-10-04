using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class FBCDepositMap : BaseClassMapping<FBCDeposit>
    {
        public FBCDepositMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.UniqueID, map => { map.NotNullable(true); map.Length(22); map.Index("IX_FBC_DEPOSIT_UNIQUEID"); });
            Property(x => x.UserID, map => { map.NotNullable(true); map.Index("IX_FBC_DEPOSIT_USERID"); });
            Property(x => x.AccountID, map => { map.NotNullable(true); });
            Property(x => x.Amount, map => { map.NotNullable(true); map.Precision(32); map.Scale(8); });
            Property(x => x.Fee, map => { map.NotNullable(true); map.Precision(16); map.Scale(8); });
            Property(x => x.ReceivePaymentTxUniqueID, map => { map.NotNullable(true); map.Length(25); map.Unique(true); });
            Property(x => x.TxID, map => { map.NotNullable(true); map.Length(70); map.Index("IX_FBC_DEPOSIT_TXID"); });
            Property(x => x.State, map => { map.NotNullable(true); });
            Property(x => x.Type, map => { map.NotNullable(true); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.VerifyAt, map => { map.NotNullable(true); });
            Property(x => x.DoneAt, map => { map.NotNullable(true); });
            Property(x => x.Memo, map => { map.NotNullable(true); map.Length(600); });
            Version(x => x.Version, map => { });
        }
    }
}
