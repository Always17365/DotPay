using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Events;
namespace DotPay.MainDomain
{

    public class SmsInterface : DomainBase, IAggregateRoot,
                                IEventHandler<SmsInterfaceCreated>,
                                IEventHandler<SmsInterfaceModified>
    {
        #region ctor
        protected SmsInterface() { }

        public SmsInterface(SmsInterfaceType smsType, string account, string password, int createBy)
        {
            this.RaiseEvent(new SmsInterfaceCreated(smsType, account, password, createBy));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual SmsInterfaceType SmsType { get; protected set; }
        public virtual string Account { get; protected set; }
        public virtual string Password { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int CreateBy { get; protected set; }
        #endregion

        #region public methods
        public virtual void Modify(SmsInterfaceType smsType, string account, string password, int byUserID)
        {
            this.RaiseEvent(new SmsInterfaceModified(smsType, account, password, byUserID));
        }
        #endregion

        #region inner events handler
        void IEventHandler<SmsInterfaceCreated>.Handle(SmsInterfaceCreated @event)
        {
            this.SmsType = @event.SmsType;
            this.Account = @event.Account;
            this.Password = @event.Password;
            this.CreateBy = @event.CreateBy;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<SmsInterfaceModified>.Handle(SmsInterfaceModified @event)
        {
            this.SmsType = @event.SmsType;
            this.Account = @event.Account;
            this.Password = @event.Password;
            this.CreateBy = @event.ByUserID;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion
    }
}
