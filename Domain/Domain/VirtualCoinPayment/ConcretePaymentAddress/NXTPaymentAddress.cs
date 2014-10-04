using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.Domain.Events;

namespace DotPay.Domain
{
    public class NXTPaymentAddress : PaymentAddress, IEventHandler<NXTPaymentAddressCreated>
    {
        #region ctor
        protected NXTPaymentAddress() { }
        public NXTPaymentAddress(int userID, int accountID, UInt64 nxtAccountID, string nxtAccountRS, string nxtPublicKey)
            : base(userID, accountID, nxtAccountRS)
        {
            this.RaiseEvent(new NXTPaymentAddressCreated(userID, accountID, nxtAccountID, nxtAccountRS, nxtPublicKey));
        }
        #endregion


        #region properties
        public virtual UInt64 NXTAccountID { get; protected set; }
        public virtual string NxtPublicKey { get; protected set; }
        #endregion

        void IEventHandler<NXTPaymentAddressCreated>.Handle(NXTPaymentAddressCreated @event)
        {
            this.UserID = @event.UserID;
            this.NXTAccountID = @event.NXTAccountID;
            this.NxtPublicKey = @event.NxtPublicKey;
        }
    }
}
