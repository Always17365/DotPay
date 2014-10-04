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
    public class VipSettingMap : BaseClassMapping<VipSetting>
    {
        public VipSettingMap()
        {
            Id(x => x.ID, map => map.Generator(Generators.Identity));

            Property(x => x.VipLevel, map => { map.NotNullable(true); });
            Property(x => x.ScoreLine, map => map.NotNullable(true));
            Property(x => x.Discount, map => { map.NotNullable(true); map.Precision(3); map.Scale(2); });
            Property(x => x.VoteCount, map => map.NotNullable(true));
            Property(x => x.CreateAt, map => map.NotNullable(true));
            Property(x => x.UpdateAt, map => map.NotNullable(true));
            Version(x => x.Version, map => { });

            DynamicUpdate(true);
        }
    }
}
