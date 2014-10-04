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

namespace DotPay.Domain.Exceptions
{
    #region state exceptions
    public class DepositNotVerifyException : DomainException
    {
        public DepositNotVerifyException() : base((int)ErrorCode.DepositNotVerify) { }
    }

    public class DepositIsVerifiedException : DomainException
    {
        public DepositIsVerifiedException() : base((int)ErrorCode.DepositIsVerified) { }
    }

    public class DepositIsCompletedException : DomainException
    {
        public DepositIsCompletedException() : base((int)ErrorCode.DepositIsCompleted) { }
    }

    public class DepositIsNotCompletedException : DomainException
    {
        public DepositIsNotCompletedException() : base((int)ErrorCode.DepositIsNotComplete) { }
    }
    #endregion

    public class DepositNotEqualsFundAmountException : DomainException
    {
        public DepositNotEqualsFundAmountException() : base((int)ErrorCode.DepositNotEqualsFundAmount) { }
    }

    public class DepositUndoCompleteForCNYException : DomainException
    {
        public DepositUndoCompleteForCNYException() : base((int)ErrorCode.AccountAmountNotEnoughBecauseUsed) { }
    }

}
