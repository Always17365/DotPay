using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Domain.Events;
using DotPay.Domain.Exceptions;
using DotPay.Common;

namespace DotPay.Domain
{
    internal class DepositCompleteStateMachine : IDepositStateMachine
    {
        private IDeposit _deposit;

        public DepositCompleteStateMachine(IDeposit deposit)
        {
            Check.Argument.IsNotNull(deposit, "deposit");

            this._deposit = deposit;
        }

        public void VerifyForCNY(int byUserID, PayWay payway, Bank bank, int fundSourceID, decimal fundAmount, string fundExtra) { throw new DepositIsVerifiedException(); }
        public void CompleteForCNY(int byUserID) { throw new DepositIsCompletedException(); }

        public void UndoCompleteForCNY(int byUserID)
        {
            var deposit = this._deposit as ICNYDeposit;

            this._deposit.RaiseEvent(new CNYDepositUndoComplete(deposit.ID, deposit.UserID, deposit.AccountID, deposit.FundSourceID, deposit.Amount, byUserID));
        }

        public void VerifyForVirtualCoin(CurrencyType currency, int byUserID, string txid, decimal txAmount) { throw new DepositIsVerifiedException(); }
        public void CompleteForVirtualCoin(CurrencyType currencyType, int byUserID) { throw new DepositIsCompletedException(); }
    }
}
