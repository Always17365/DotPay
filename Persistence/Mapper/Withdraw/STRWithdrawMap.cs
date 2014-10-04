using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class STRWithdrawMap : BaseClassMapping<STRWithdraw>
    {
        public STRWithdrawMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.UserID, map => { map.NotNullable(true); map.Index("IX_STR_WITHDRAW_USERID"); });
            Property(x => x.UniqueID, map => { map.NotNullable(true); map.Length(24); map.Index("IX_STR_WITHDRAW_UNIQUE_ID"); });
            Property(x => x.AccountID, map => { map.NotNullable(true); });
            Property(x => x.TxID, map => { map.NotNullable(true); map.Length(67); map.Index("IX_STR_WITHDRAW_TXID"); });
            Property(x => x.ReceiveAddress, map => { map.NotNullable(true); map.Index("IX_STR_WITHDRAW_RECEIVE_ADDRESS"); });
            Property(x => x.State, map => { map.NotNullable(true); });
            Property(x => x.Amount, map => { map.NotNullable(true); map.Precision(24); map.Scale(16); });
            Property(x => x.Fee, map => { map.NotNullable(true); map.Precision(24); map.Scale(16); });
            Property(x => x.TxFee, map => { map.NotNullable(true); map.Precision(24); map.Scale(16); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.VerifyAt, map => { map.NotNullable(true); });
            Property(x => x.DoneAt, map => { map.NotNullable(true); });
            Property(x => x.FailAt, map => { map.NotNullable(true); });
            Property(x => x.Memo, map => { map.NotNullable(true); map.Length(600); });
            Version(x => x.Version, map => { });
        }
    }
}
