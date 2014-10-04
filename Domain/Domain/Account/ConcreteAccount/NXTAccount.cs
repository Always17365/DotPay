using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Domain.Events;
using DotPay.Domain.Exceptions;
using DotPay.Common;

namespace DotPay.Domain
{
    public class NXTAccount : Account
    {
        #region ctor
        protected NXTAccount() { }

        public NXTAccount(int userID) : base(userID) { }
        #endregion
    }
}
