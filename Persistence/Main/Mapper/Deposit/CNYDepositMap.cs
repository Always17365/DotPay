using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class CNYDepositMap : BaseClassMapping<CNYDeposit>
    {
        public CNYDepositMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));
             
            Property(x => x.UserID, map => { map.NotNullable(true); map.Index("IX_CNY_DEPOSIT_USERID"); });
            Property(x => x.AccountID, map => { map.NotNullable(true); });
            Property(x => x.Amount, map => { map.NotNullable(true); map.Precision(24); map.Scale(2); }); 
            Property(x => x.PayWay, map => { map.NotNullable(true); map.Precision(24); map.Scale(2); });
            Property(x => x.State, map => { map.NotNullable(true); }); 
            Property(x => x.CreateAt, map => { map.NotNullable(true); }); 
            Property(x => x.DoneAt, map => { map.NotNullable(true); });
            Property(x => x.Memo, map => { map.NotNullable(true); map.Length(600); });
            Version(x => x.Version, map => { });
        }
    }
}
