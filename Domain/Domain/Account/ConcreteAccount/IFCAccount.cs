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
    public class IFCAccount : Account
    {
        #region ctor
        protected IFCAccount() { }

        public IFCAccount(int userID) : base(userID) { }
        #endregion
    }
}
