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
    public class ToTenpayTransferTransaction : InboundTransferToThirdPartyPaymentTx
    {

        #region ctor
        protected ToTenpayTransferTransaction() { }

        public ToTenpayTransferTransaction(string txid, string account, decimal amount)
            : base(txid, account, amount, PayWay.Tenpay)
        {

        }
        #endregion
    }
}
