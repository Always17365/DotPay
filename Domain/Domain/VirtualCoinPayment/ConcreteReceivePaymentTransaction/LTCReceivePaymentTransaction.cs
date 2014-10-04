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
    public class LTCReceivePaymentTransaction : ReceivePaymentTransaction
    {
        #region ctor
        protected LTCReceivePaymentTransaction() { }
        public LTCReceivePaymentTransaction(string txid, string address, decimal amount)
        {
            this.RaiseEvent(new PaymentTransactionCreated(txid, address, CurrencyType.LTC, amount, this));
        }
        #endregion

        #region public method
        public override void Confirm(int comfirmations, string txid)
        {
            if (this.State == VirtualCoinTxState.Complete)
                throw new PaymentTransactionIsCompletedException();

            this.RaiseEvent(new PaymentTransactionConfirmed(this.ID, txid, comfirmations, CurrencyType.LTC));
        }
        #endregion
    }
}
