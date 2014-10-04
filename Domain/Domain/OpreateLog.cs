using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using DotPay.Domain.Events;
using FC.Framework;

namespace DotPay.Domain
{
    public abstract class OperateLog : DomainBase, IAggregateRoot,
                            IEventHandler<OperateLogCreated>
    {
        #region ctor
        protected OperateLog() { }

        public OperateLog(int domainID, string uniqueID, string memo, int operatorID, string ip)
        {
            this.RaiseEvent(new OperateLogCreated(domainID, uniqueID, memo, operatorID, ip));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int DomainID { get; protected set; }
        public virtual string UniqueID { get; protected set; }
        public virtual int OperatorID { get; protected set; }
        public virtual int OperateTime { get; protected set; }
        public virtual string Memo { get; protected set; }
        public virtual string IP { get; protected set; }
        #endregion

        #region inner event handlers
        void IEventHandler<OperateLogCreated>.Handle(OperateLogCreated @event)
        {
            this.DomainID = @event.DomainID;
            this.UniqueID = @event.UniqueID;
            this.Memo = @event.Memo;
            this.IP = @event.IP;
            this.OperatorID = @event.OperateUserID;
            this.OperateTime = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion
    }
}
