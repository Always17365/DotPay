using System.Linq;
using DotPay.MainDomain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using DotPay.Common;

namespace DotPay.Persistence
{
    public class MembershipMap : BaseClassMapping<Membership>
    {
        public MembershipMap()
        {
            Id(x => x.UserID, map => { map.Generator(Generators.Foreign<Membership>(membership => membership.Owner)); });

            Property(x => x.Email, map => map.NotNullable(true));
            Property(x => x.IdNo, map => { map.Length(20); map.NotNullable(true); });
            Property(x => x.IdNoType, map => map.NotNullable(true));
            Property(x => x.RealName, map => { map.Length(30); map.NotNullable(true); });
            Property(x => x.RegisterAt, map => map.NotNullable(true));
            Property(x => x.LastPasswordVerifyAt, map => map.NotNullable(true));
            Property(x => x.CreateAt, map => map.NotNullable(true));
            Property(x => x.IsEmailVerify, map => map.NotNullable(true));
            Property(x => x.EmailValidateToken, map => map.NotNullable(true));
            Property(x => x.IsLocked, map => map.NotNullable(true));
            Property(x => x.LockAt, map => map.NotNullable(true));
            Property(x => x.UnLockAt, map => map.NotNullable(true));

            Property(x => x.Password, map => { map.Length(32); map.NotNullable(true); });
            Property(x => x.LastPasswordVerifyAt, map => map.NotNullable(true));
            Property(x => x.LastReceiveScoreAt, map => map.NotNullable(true));
            Property(x => x.LastPasswordFailureAt, map => map.NotNullable(true));
            Property(x => x.PasswordChangeAt, map => map.NotNullable(true));
            Property(x => x.PasswordResetToken, map => map.NotNullable(true));

            Property(x => x.TradePassword, map => { map.Length(32); map.NotNullable(true); });
            Property(x => x.LastTradePasswordVerifyAt, map => map.NotNullable(true));
            Property(x => x.LastTradePasswordFailureAt, map => map.NotNullable(true));
            Property(x => x.TradePasswordChangeAt, map => map.NotNullable(true));
            Property(x => x.TradePasswordResetToken, map => map.NotNullable(true));
            Version(x => x.Version, map => { });

            DynamicUpdate(true);
            OneToOne(x => x.Owner, map =>
            {
                map.Cascade(Cascade.All);
            });
        }

    }
}
