using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class CNYWithdrawMap : BaseClassMapping<CNYWithdraw>
    {
        public CNYWithdrawMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.UserID, map => { map.NotNullable(true); map.Index("IX_CNY_WITHDRAW_USERID"); });
            Property(x => x.UniqueID, map => { map.NotNullable(true); map.Length(24); map.Index("IX_CNY_WITHDRAW_UNIQUE_ID"); });
            Property(x => x.AccountID, map => { map.NotNullable(true); });
            Property(x => x.TransferAccountID, map => { map.NotNullable(true); });
            Property(x => x.TransferNo, map => { map.NotNullable(true); map.Length(40); map.Index("IX_CNY_WITHDRAW_TRANSFER_NO"); });
            Property(x => x.ReceiverBankAccountID, map => { map.NotNullable(true); });
            Property(x => x.State, map => { map.NotNullable(true); });
            Property(x => x.Amount, map => { map.NotNullable(true); map.Precision(16); map.Scale(2); });
            Property(x => x.Fee, map => { map.NotNullable(true); map.Precision(8); map.Scale(2); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); }); 
            Property(x => x.SubmitAt, map => { map.NotNullable(true); });
            Property(x => x.VerifyAt, map => { map.NotNullable(true); });
            Property(x => x.DoneAt, map => { map.NotNullable(true); });
            Property(x => x.Memo, map => { map.NotNullable(true); map.Length(600); });
            Version(x => x.Version, map => { });
        }
    }
}
