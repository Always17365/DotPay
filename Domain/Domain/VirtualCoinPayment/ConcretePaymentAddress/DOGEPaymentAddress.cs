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
    public class DOGEPaymentAddress : PaymentAddress
    {
        #region ctor
        protected DOGEPaymentAddress() { }
        public DOGEPaymentAddress(int userID, int accountID, string address)
            : base(userID, accountID, address) { }
        #endregion
    }
}
