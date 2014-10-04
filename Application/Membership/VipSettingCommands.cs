using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region set vip level discount command
    [ExecuteSync]
    public class SetVipLevelDiscount : FC.Framework.Command
    {
        public SetVipLevelDiscount(int vipLevelId, decimal discount, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(vipLevelId, "vipLevelId");
            Check.Argument.IsNotNegative(discount, "discount");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.VipLevelId = vipLevelId;
            this.Discount = discount;
            this.ByUserID = byUserID;
        }

        public int VipLevelId { get; private set; }
        public decimal Discount { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region set vip level score line command
    [ExecuteSync]
    public class SetVipLevelScoreLine : FC.Framework.Command
    {
        public SetVipLevelScoreLine(int vipLevelId, int scoreLine, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(vipLevelId, "vipLevelId");
            Check.Argument.IsNotNegative(scoreLine, "scoreLine");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.VipLevelId = vipLevelId;
            this.ScoreLine = scoreLine;
            this.ByUserID = byUserID;
        }

        public int VipLevelId { get; private set; }
        public int ScoreLine { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region set vip level vote count command
    [ExecuteSync]
    public class SetVipLevelVoteCount : FC.Framework.Command
    {
        public SetVipLevelVoteCount(int vipLevelId, int voteCount, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(vipLevelId, "vipLevelId");
            Check.Argument.IsNotNegative(voteCount, "discount");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.VipLevelId = vipLevelId;
            this.VoteCount = voteCount;
            this.ByUserID = byUserID;
        }

        public int VipLevelId { get; private set; }
        public int VoteCount { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion
}
