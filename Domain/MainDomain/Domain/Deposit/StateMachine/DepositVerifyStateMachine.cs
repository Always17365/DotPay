using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Exceptions;
using DotPay.Common;

namespace DotPay.MainDomain
{
    internal class DepositVerifyStateMachine : IDepositStateMachine
    {
        private IDeposit _deposit;

        public DepositVerifyStateMachine(IDeposit deposit)
        {
            Check.Argument.IsNotNull(deposit, "deposit");

            this._deposit = deposit;
        }

        public void VerifyForCNY(int byUserID, PayWay payway, Bank bank, int fundSourceID, decimal fundAmount) { throw new DepositIsVerifiedException(); }

        public void CompleteForCNY(int byUserID)
        {
            this._deposit.RaiseEvent(new CNYDepositCompleted(this._deposit.ID, this._deposit.UserID, this._deposit.AccountID, this._deposit.Amount, byUserID));
        }

        public void UndoCompleteForCNY(int byUserID) { throw new DepositIsNotCompletedException(); }
        public void VerifyForVirtualCoin(CurrencyType currency, int byUserID, string txid, decimal txAmount) { throw new DepositIsVerifiedException(); }

        public void CompleteForVirtualCoin(CurrencyType currencyType,int byUserID)
        {
            this._deposit.RaiseEvent(new VirtualCoinDepositCompleted(this._deposit.ID, this._deposit.UserID, this._deposit.AccountID, this._deposit.Amount,currencyType, byUserID));
        }
    }
}
