using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.Domain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.Domain.Exceptions;

namespace DotPay.Domain
{
    public class FBCDeposit : VirtualCoinDeposit
    {
        #region ctor
        protected FBCDeposit() : base() { }

        public FBCDeposit(int userID, string receivePaymentTxUniqueID, int accountID, string txid, decimal amount, decimal fee, string memo, DepositType depositType) :
            base(userID, accountID, txid, CurrencyType.FBC, amount, fee, memo, depositType)
        {
            this.ReceivePaymentTxUniqueID = receivePaymentTxUniqueID;
        }
        #endregion

        public virtual string ReceivePaymentTxUniqueID { get; protected set; }
        public override void Complete(CurrencyType currencyType, int byUserID)
        {
            this.StateMachine.CompleteForVirtualCoin(CurrencyType.FBC, byUserID);
        }

        public override void VerifyAmount(int byUserID, string txid, decimal txAmount)
        {
            if (txAmount != (this.Amount + this.Fee))
                throw new DepositNotEqualsFundAmountException();

            this.StateMachine.VerifyForVirtualCoin(CurrencyType.FBC, byUserID, txid, txAmount);
        }
    }
}
