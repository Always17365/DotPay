using FC.Framework;
using FC.Framework.NHibernate;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.Domain;
using DotPay.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Persistence.Repository
{
    public class VipSettingRepository : FC.Framework.NHibernate.Repository, IVipSettingRepository
    {
        public VipSettingRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public VipSetting FindByVipLevel(UserVipLevel vipLevel)
        {
            //var cacheKey = CacheKey.VIP_SETTING_KEY + "DOMAIN_" + vipLevel.ToString("D");
            //VipSetting vipSetting;

            //if (!Cache.TryGet<VipSetting>(cacheKey, out vipSetting))
            var vipSetting = base.FindById<VipSetting>((int)(vipLevel));

            return vipSetting;
        }
    }
}
