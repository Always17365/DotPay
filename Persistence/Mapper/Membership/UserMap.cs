using DotPay.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class UserMap : BaseClassMapping<User>
    {
        public UserMap()
        {
            Id(u => u.ID, map => { map.Generator(Generators.Identity); });

            Property(x => x.NickName, map => { map.Length(70); map.NotNullable(true); });
            Property(x => x.Email, map => { map.NotNullable(true); map.Index("IX_USER_EMAIL"); });
            Property(x => x.VipLevel, map => map.NotNullable(true));
            Property(x => x.IsOpenAuth, map => map.NotNullable(true));
            Property(x => x.CommendCounter, map => { map.NotNullable(true); });
            Property(x => x.CommendBy, map => { map.NotNullable(true); });
            Property(x => x.Role, map => map.NotNullable(true));
            Property(x => x.RippleAddress, map => map.NotNullable(true));
            Property(x => x.RippleSecret, map => map.NotNullable(true));
            Property(x => x.TimeZone, map => map.NotNullable(true));
            Property(x => x.TwoFactorFlg, map => map.NotNullable(true));
            Property(x => x.Mobile, map => { map.Length(20); map.NotNullable(true); });
            Property(x => x.VipLevel, map => map.NotNullable(true));
            Property(x => x.ScoreBalance, map => map.NotNullable(true));
            Property(x => x.ScoreUsed, map => map.NotNullable(true));
            Property(x => x.CreateAt, map => map.NotNullable(true));
            Property(x => x.UpdateAt, map => map.NotNullable(true));
            Version(x => x.Version, map => { });

            OneToOne(x => x.Membership, map => { map.Cascade(Cascade.All); map.Lazy(LazyRelation.Proxy); });
            OneToOne(x => x.GoogleAuthentication, map => { map.Cascade(Cascade.All); map.Lazy(LazyRelation.Proxy); });
            OneToOne(x => x.SmsAuthentication, map => { map.Cascade(Cascade.All); map.Lazy(LazyRelation.Proxy); });

            DynamicUpdate(true);
        }
    }
}
