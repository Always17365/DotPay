using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Exceptions;
using DotPay.Common;

namespace DotPay.MainDomain
{
    public class CNYAccount : Account
    {
        #region ctor
        protected CNYAccount() { }

        public CNYAccount(int userID) : base(userID) { }
        #endregion
    }
}
