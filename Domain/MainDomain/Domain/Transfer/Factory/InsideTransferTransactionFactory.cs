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
    public class InsideTransferTransactionFactory
    {
        public static InsideTransferTransaction CreateInsideTransferTransaction(int fromUserID, int toUserID,
                                    CurrencyType currency, decimal amount, PayWay payway, string description)
        {
            InsideTransferTransaction tx;

            switch (currency)
            {
                case CurrencyType.CNY:
                    tx = new CNYInsideTransferTransaction(fromUserID, toUserID, amount, payway, description);
                    break;
                case CurrencyType.USD:
                    tx = new USDInsideTransferTransaction(fromUserID, toUserID, amount, payway, description);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return tx;
        }
    }
}
