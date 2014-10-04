using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class LTCAccountVersionMap : BaseClassMapping<LTCAccountVersion>
    {
        public LTCAccountVersionMap()
        {
            Id(u => u.ID, map => map.Generator(Generators.Identity));

            Property(x => x.UserID, map => { map.NotNullable(true); });
            Property(x => x.AccountID, map => { map.NotNullable(true); map.Index("IX_LTC_ACCOUNT_VERSION_ACCOUNT_ID"); }); 
            Property(x => x.Amount, map => { map.NotNullable(true); map.Precision(32); map.Scale(16); });
            Property(x => x.Balance, map => { map.NotNullable(true); map.Precision(32); map.Scale(16); });
            Property(x => x.Locked, map => { map.NotNullable(true); map.Precision(32); map.Scale(16); });
            Property(x => x.In, map => { map.NotNullable(true); map.Column("`In`"); map.Precision(32); map.Scale(16); });
            Property(x => x.Out, map => { map.NotNullable(true); map.Column("`Out`"); map.Precision(32); map.Scale(16); });
            Property(x => x.CreateAt, map => { map.NotNullable(true); });
            Property(x => x.ModifyID, map => { map.NotNullable(true); });
            Property(x => x.ModifyUniqueID, map => { map.NotNullable(true); map.Length(30); });
            Property(x => x.ModifyType, map => { map.NotNullable(true); });
            Version(x => x.Version, map => { });
        }
    }
}
