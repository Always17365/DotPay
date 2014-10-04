using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using DotPay.ViewModel;
using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;

namespace DotPay.QueryService.Impl
{
    public class VipSettingQuery : AbstractQuery, IVipSettingQuery
    {
        public IEnumerable<DotPay.ViewModel.VipSettingListModel> GetVipSettingBySearch()
        {
            var users = this.Context.Sql(getVipSettingBySearch_sql)
                                   .QueryMany<VipSettingListModel>();

            return users;
        }
        private readonly string getVipSettingBySearch_sql =
                        @"SELECT   ID,VipLevel,ScoreLine,Discount,VoteCount,CreateAt,UpdateAt
                                    FROM   " + Config.Table_Prefix + @"vipsetting";
    }
}
