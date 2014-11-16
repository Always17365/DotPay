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
    public class CNYInsideTransferTransaction : InsideTransferTransaction
    { 

        #region ctor
        protected CNYInsideTransferTransaction() { }

        public CNYInsideTransferTransaction(int fromUserID, int toUserID, decimal amount, PayWay payway, string description)
            : base(fromUserID, toUserID, CurrencyType.CNY, amount, payway, description)
        {

        }
        #endregion
    }
}
