using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.Domain.Events;
using DotPay.Domain.Exceptions;

namespace DotPay.Domain
{
    public class STRReceivePaymentTransaction : ReceivePaymentTransaction
    {
        #region ctor
        protected STRReceivePaymentTransaction() { }
        public STRReceivePaymentTransaction(int userID, string txid, string receiveParamter, decimal amount)
        {
            this.UserID = userID;//未来开通自动充值，此字段要去除
            this.RaiseEvent(new PaymentTransactionCreated(txid, receiveParamter, CurrencyType.STR, amount, this, userID));
        }
        #endregion

        #region public method
        public override void Confirm(int comfirmations, string txid)
        {
            if (this.State == VirtualCoinTxState.Complete)
                throw new PaymentTransactionIsCompletedException();

            this.RaiseEvent(new PaymentTransactionConfirmed(this.ID, txid, comfirmations, CurrencyType.STR));
        }
        #endregion
    }
}
