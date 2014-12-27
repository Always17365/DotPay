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
    public class ToAlipayTransferTransaction : InboundTransferToThirdPartyPaymentTx
    {

        #region ctor
        protected ToAlipayTransferTransaction() { }

        public ToAlipayTransferTransaction(string txid, string account, decimal amount, PayWay sourcePayway, string realName, string memo)
            : base(txid, account, amount, PayWay.Alipay, sourcePayway, realName, memo)
        {

        }
        #endregion
    }
}
