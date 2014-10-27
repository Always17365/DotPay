using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using DotPay.MainDomain;

namespace DotPay.Persistence
{
    public class GoogleAuthenticationMap : BaseClassMapping<GoogleAuthentication>
    {
        public GoogleAuthenticationMap()
        {
            Id(x => x.UserID, map => map.Generator(Generators.Assigned));

            Property(x => x.OTPSecret, map => { map.Length(16); map.NotNullable(true); });
            Property(x => x.LastVerifyAt, map => map.NotNullable(true));
            Property(x => x.LastFailureAt, map => map.NotNullable(true));
            Property(x => x.UpdateAt, map => map.NotNullable(true));
            Property(x => x.CreateAt, map => map.NotNullable(true));
            Version(x => x.Version, map => { }); 
        }
    }

    public class SmsAuthenticationMap : BaseClassMapping<SmsAuthentication>
    {
        public SmsAuthenticationMap()
        {
            Id(x => x.UserID, map => map.Generator(Generators.Assigned));

            Property(x => x.UserID);
            Property(x => x.OTPSecret, map => { map.Length(16); map.NotNullable(true); });
            Property(x => x.LastVerifyAt, map => map.NotNullable(true));
            Property(x => x.LastFailureAt, map => map.NotNullable(true));
            Property(x => x.SmsCounter, map => map.NotNullable(true));
            Property(x => x.UpdateAt, map => map.NotNullable(true));
            Property(x => x.CreateAt, map => map.NotNullable(true));
            Version(x => x.Version, map => { }); 
        }
    }

}
