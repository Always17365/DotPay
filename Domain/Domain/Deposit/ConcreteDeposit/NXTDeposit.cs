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
    public class NXTDeposit : VirtualCoinDeposit
    {
        #region ctor
        protected NXTDeposit() : base() { }

        public NXTDeposit(int userID, int accountID, string txid, decimal amount, decimal fee, string memo, DepositType depositType) :
            base(userID, accountID, txid, CurrencyType.NXT, amount, fee, memo, depositType) { }
        #endregion

        public override void Complete(CurrencyType currencyType, int byUserID)
        {
            this.StateMachine.CompleteForVirtualCoin(CurrencyType.NXT, byUserID);
        }

        public override void VerifyAmount(int byUserID, string txid, decimal txAmount)
        {
            if (txAmount != (this.Amount + this.Fee))
                throw new DepositNotEqualsFundAmountException();

            this.StateMachine.VerifyForVirtualCoin(CurrencyType.NXT, byUserID, txid, txAmount);
        }
    }
}
