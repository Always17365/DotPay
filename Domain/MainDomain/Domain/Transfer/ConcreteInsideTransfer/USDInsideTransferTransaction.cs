using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.MainDomain.Exceptions;

namespace DotPay.MainDomain
{
    public class USDInsideTransferTransaction : InsideTransferTransaction
    {
        #region ctor
        protected USDInsideTransferTransaction() { }

        public USDInsideTransferTransaction(int fromUserID, int toUserID, decimal amount, PayWay payway, string description)
            : base(fromUserID, toUserID, CurrencyType.USD, amount, payway, description)
        {
        }
        #endregion

    }
}
