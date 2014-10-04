using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.Domain.Exceptions;
using DotPay.Domain.Events;

namespace DotPay.Domain
{
    public class VipSetting : DomainBase, IAggregateRoot,
                      IEventHandler<VipSettingModifyScoreLine>,
                      IEventHandler<VipSettingModifyDiscount>,
                      IEventHandler<VipSettingModifyVoteCount>
    {
        #region ctor
        protected VipSetting() { }

        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual UserVipLevel VipLevel { get; protected set; }
        public virtual int ScoreLine { get; protected set; }
        public virtual decimal Discount { get; protected set; }
        public virtual int VoteCount { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int UpdateAt { get; protected set; }
        #endregion

        #region public method
        public virtual void ModifyScoreLine(int scoreLine, int byUserID)
        {
            this.RaiseEvent(new VipSettingModifyScoreLine(this.VipLevel, scoreLine, byUserID));
        }

        public virtual void ModifyDiscount(decimal discount, int byUserID)
        {
            this.RaiseEvent(new VipSettingModifyDiscount(this.VipLevel, discount, byUserID));
        }

        public virtual void ModifyVoteCount(int voteCount, int byUserID)
        {
            this.RaiseEvent(new VipSettingModifyVoteCount(this.VipLevel, voteCount, byUserID));
        }
        #endregion

        #region inner event handlers
        void IEventHandler<VipSettingModifyDiscount>.Handle(VipSettingModifyDiscount @event)
        {
            this.Discount = @event.Discount;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        void IEventHandler<VipSettingModifyScoreLine>.Handle(VipSettingModifyScoreLine @event)
        {
            this.ScoreLine = @event.ScoreLine;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<VipSettingModifyVoteCount>.Handle(VipSettingModifyVoteCount @event)
        {
            this.VoteCount = @event.VoteCount;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion

    }
}
