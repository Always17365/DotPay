using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain
{
    internal class DepositStateMachineFactory
    {
        public static IDepositStateMachine CreateStateMachine(IDeposit deposit)
        {
            switch (deposit.State)
            {
                case DepositState.Pending:
                    return new DepositWaitProcessStateMachine(deposit);
                case DepositState.Verify:
                    return new DepositVerifyStateMachine(deposit);
                case DepositState.Complete:
                    return new DepositCompleteStateMachine(deposit);
                default: return null;
            }
        }
    }
}
