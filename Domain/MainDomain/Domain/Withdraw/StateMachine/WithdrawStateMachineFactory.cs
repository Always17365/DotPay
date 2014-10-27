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
    internal class WithdrawStateMachineFactory
    {
        public static IWithdrawStateMachine CreateStateMachine(IWithdraw withdraw)
        {
            var statemachine = default(IWithdrawStateMachine);

            switch (withdraw.State)
            {
                case WithdrawState.WaitVerify:
                    statemachine = new WithdrawWaitVerifyStateMachine(withdraw);
                    break;
                case WithdrawState.WaitSubmit:
                    statemachine = new WithdrawWaitSubmitStateMachine(withdraw);
                    break;
                case WithdrawState.Processing:
                    statemachine = new WithdrawProcessingStateMachine(withdraw);
                    break;
                case WithdrawState.Complete:
                    statemachine = new WithdrawCompleteStateMachine(withdraw);
                    break;
                case WithdrawState.Fail:
                    statemachine = new WithdrawFailStateMachine(withdraw);
                    break;
                case WithdrawState.Cancel:
                    throw new WithdrawIsCanceledException();
                default: throw new NotImplementedException();
            }
            return statemachine;
        }
    }
}
