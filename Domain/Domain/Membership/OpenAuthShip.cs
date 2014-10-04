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
    public class OpenAuthShip : DomainBase, IAggregateRoot,
                                IEventHandler<OpenAuthShipCreated>
    {
        #region ctor
        protected OpenAuthShip() { }

        public OpenAuthShip(int userID, string openID, OpenAuthType openAuthType)
        {
            this.RaiseEvent(new OpenAuthShipCreated(userID, openID, openAuthType));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual string OpenID { get; protected set; }
        public virtual OpenAuthType Type { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        #endregion

        void IEventHandler<OpenAuthShipCreated>.Handle(OpenAuthShipCreated @event)
        {
            this.UserID = @event.UserID;
            this.OpenID = @event.OpenID;
            this.Type = @event.OpenAuthType;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
    }
}
