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
        public static InboundTransferToThirdPartyPaymentTx CreateInboundTransferTransaction(string txid, string account, decimal amount, PayWay payway, PayWay sourcePayway, string realName, string memo)
        {
            InboundTransferToThirdPartyPaymentTx tx;

            switch (payway)
            {
                case PayWay.Alipay:
                    tx = new ToAlipayTransferTransaction(txid, account, amount, sourcePayway, realName, memo);
                    break;
                case PayWay.Tenpay:
                    tx = new ToTenpayTransferTransaction(txid, account, amount, sourcePayway, realName, memo);
                    break;
                default:
                    tx = new ToBankTransferTransaction(txid, account, amount, sourcePayway, payway, realName, memo);
                    break;
            }

            return tx;
        }
    }
}
