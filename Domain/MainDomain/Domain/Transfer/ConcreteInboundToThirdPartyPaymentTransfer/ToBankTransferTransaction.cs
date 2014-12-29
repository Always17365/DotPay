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
    public class ToBankTransferTransaction : InboundTransferToThirdPartyPaymentTx
    {

        #region ctor
        protected ToBankTransferTransaction() { }

        public ToBankTransferTransaction(string txid, string account, decimal amount, PayWay sourcePayway, PayWay bank, string realName, string memo)
            : base(txid, account, amount, bank, sourcePayway, realName, memo)
        {

        }
        #endregion
    }
}
