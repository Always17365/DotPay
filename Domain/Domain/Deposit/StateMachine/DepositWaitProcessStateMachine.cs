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
    internal class DepositWaitProcessStateMachine : IDepositStateMachine
    {
        private IDeposit _deposit;

        public DepositWaitProcessStateMachine(IDeposit deposit)
        {
            Check.Argument.IsNotNull(deposit, "deposit");

            this._deposit = deposit;
        }
        public void VerifyForCNY(int byUserID, PayWay payway, Bank bank, int fundSourceID, decimal fundAmount, string fundExtra)
        {
            this._deposit.RaiseEvent(new CNYDepositVerified(this._deposit.ID, payway, bank, this._deposit.UniqueID, byUserID, fundSourceID, fundAmount, fundExtra));
        }

        public void CompleteForCNY(int byUserID) { throw new DepositNotVerifyException(); }
        public void UndoCompleteForCNY(int byUserID) { throw new DepositIsNotCompletedException(); }

        public void VerifyForVirtualCoin(CurrencyType currency, int byUserID, string txid, decimal txAmount)
        {
            this._deposit.RaiseEvent(new VirtualCoinDepositVerified(currency, this._deposit.ID, this._deposit.UniqueID, byUserID, txid, txAmount));
        }
        public void CompleteForVirtualCoin(CurrencyType currency, int byUserID) { throw new DepositNotVerifyException(); }
    }
}
