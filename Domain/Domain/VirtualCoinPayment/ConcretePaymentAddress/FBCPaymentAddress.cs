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
    public class FBCPaymentAddress : PaymentAddress
    {
        #region ctor
        protected FBCPaymentAddress() { }
        public FBCPaymentAddress(int userID, int accountID, string address)
            : base(userID, accountID, address) { }
        #endregion
    }
}
