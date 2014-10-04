using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using DotPay.Domain;

namespace DotPay.Persistence
{
    public class TokenMap : BaseClassMapping<Token>
    {
        public TokenMap()
        {
            Id(x => x.ID, map => map.Generator(Generators.Identity));

            Property(x => x.UserID, map => { map.NotNullable(true); });
            Property(x => x.Value, map => map.NotNullable(true));
            Property(x => x.Type, map => map.NotNullable(true));
            Property(x => x.IsUsed, map => map.NotNullable(true));
            Property(x => x.ExpiredAt, map => map.NotNullable(true));
            Property(x => x.CreateAt, map => map.NotNullable(true));
            Property(x => x.UpdateAt, map => map.NotNullable(true));
            Version(x => x.Version, map => { });

            DynamicUpdate(true);
        }
    }
}
