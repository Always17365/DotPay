using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class WithdrawReceiverBankAccountMap : BaseClassMapping<WithdrawReceiverBankAccount>
    {
        public WithdrawReceiverBankAccountMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.UserID, map => { map.NotNullable(true); map.Index("IX_WITHDRAW_RECEIVER_BANK_ACCOUNT_USERID"); });
            Property(x => x.Valid);
            Property(x => x.Bank, map => { map.NotNullable(true); });
            //Property(x => x.BankOutletsID, map => { map.NotNullable(true); });
            Property(x => x.BankAccount, map => { map.NotNullable(true); map.Length(20); });
            Property(x => x.ReceiverName, map => { map.NotNullable(true); map.Length(30); });
            Property(x => x.MarkBy, map => { map.NotNullable(true); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.MarkAt, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });

            DynamicInsert(true);
        }
    }
}
