using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.MainDomain.Events;

namespace DotPay.MainDomain
{
    public abstract class PaymentAddress : DomainBase, IAggregateRoot,
                                           IEventHandler<PaymentAddressCreated>
    {
        #region ctor
        protected PaymentAddress() { }
        public PaymentAddress(int userID, int accountID, string address)
        {
            this.RaiseEvent(new PaymentAddressCreated(userID, accountID, address));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual int AccountID { get; protected set; }
        public virtual string Address { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        #endregion

        void IEventHandler<PaymentAddressCreated>.Handle(PaymentAddressCreated @event)
        {
            this.UserID = @event.UserID;
            this.AccountID = @event.AccountID;
            this.Address = @event.Address;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
    }
}
