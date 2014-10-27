using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{
    #region manager

    public class VipSettingModifyScoreLine : DomainEvent
    {
        public VipSettingModifyScoreLine(UserVipLevel vipSettingLevel, int scoreLine, int byUserID)
        {
            this.UserVipLevel = vipSettingLevel;
            this.ScoreLine = scoreLine;
            this.ByUserID = byUserID;
        }
        public UserVipLevel UserVipLevel { get; protected set; }
        public int ByUserID { get; protected set; }
        public int ScoreLine { get; protected set; }
    }

    public class VipSettingModifyDiscount : DomainEvent
    {
        public VipSettingModifyDiscount(UserVipLevel vipSettingLevel, decimal discount, int byUserID)
        {
            this.UserVipLevel = vipSettingLevel;
            this.Discount = discount;
            this.ByUserID = byUserID;
        }
        public UserVipLevel UserVipLevel { get; protected set; }
        public int ByUserID { get; protected set; }
        public decimal Discount { get; protected set; }
    }

    public class VipSettingModifyVoteCount : DomainEvent
    {
        public VipSettingModifyVoteCount(UserVipLevel vipSettingLevel, int voteCount, int byUserID)
        {
            this.UserVipLevel = vipSettingLevel;
            this.VoteCount = voteCount;
            this.ByUserID = byUserID;
        }
        public UserVipLevel UserVipLevel { get; protected set; }
        public int ByUserID { get; protected set; }
        public int VoteCount { get; protected set; }
    }
    #endregion
}
