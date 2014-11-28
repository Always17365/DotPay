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
    public class ToThirdPartyPaymentTransactionFactory
    {
        public static InboundTransferToThirdPartyPaymentTx CreateInsideTransferTransaction(string txid, string account, decimal amount, PayWay payway)
        {
            InboundTransferToThirdPartyPaymentTx tx;

            switch (payway)
            {
                case PayWay.Alipay:
                    tx = new ToAlipayTransferTransaction(txid, account, amount);
                    break;
                case PayWay.Tenpay:
                    tx = new ToTenpayTransferTransaction(txid, account, amount);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return tx;
        }
    }
}
