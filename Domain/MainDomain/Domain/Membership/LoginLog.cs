using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using DotPay.MainDomain.Events;
using FC.Framework;

namespace DotPay.MainDomain
{
    public class LoginLog : DomainBase, IAggregateRoot,
                            IEventHandler<LoginLogCreated>
    {
        #region ctor
        protected LoginLog() { }

        public LoginLog(int userID, string ip)
        {
            this.RaiseEvent(new LoginLogCreated(userID, ip));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual int LoginTime { get; protected set; }
        public virtual string IP { get; protected set; }
        #endregion

        void IEventHandler<LoginLogCreated>.Handle(LoginLogCreated @event)
        {
            this.UserID = @event.UserID;
            this.LoginTime = @event.UTCTimestamp.ToUnixTimestamp();
            this.IP = @event.IP;
        }
    }
}
