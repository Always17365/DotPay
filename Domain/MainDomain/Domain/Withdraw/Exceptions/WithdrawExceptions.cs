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

namespace DotPay.MainDomain.Exceptions
{
    #region state exceptions
    public class WithdrawIsNotVerifyException : DomainException
    {
        public WithdrawIsNotVerifyException() : base((int)ErrorCode.WithdrawNotVerify) { }
    }

    public class WithdrawIsVerifiedException : DomainException
    {
        public WithdrawIsVerifiedException() : base((int)ErrorCode.WithdrawIsVerified) { }
    }

    public class WithdrawIsNotSubmitToProcessException : DomainException
    {
        public WithdrawIsNotSubmitToProcessException() : base((int)ErrorCode.WithdrawIsNotSubmitToProcess) { }
    }

    public class WithdrawIsProcessingException : DomainException
    {
        public WithdrawIsProcessingException() : base((int)ErrorCode.WithdrawIsProcessing) { }
    }

    public class WithdrawIsCompletedException : DomainException
    {
        public WithdrawIsCompletedException() : base((int)ErrorCode.WithdrawIsCompleted) { }
    }

    public class WithdrawIsNotCompleteException : DomainException
    {
        public WithdrawIsNotCompleteException() : base((int)ErrorCode.WithdrawIsNotComplete) { }
    }

    public class WithdrawIsFailedException : DomainException
    {
        public WithdrawIsFailedException() : base((int)ErrorCode.WithdrawIsFailed) { }
    }

    public class WithdrawIsCanceledException : DomainException
    {
        public WithdrawIsCanceledException() : base((int)ErrorCode.WithdrawIsCanceled) { }
    }

    #endregion
     public class WithdrawOverDayLimitException : DomainException
    {
         public WithdrawOverDayLimitException() : base((int)ErrorCode.WithdrawIsOverDayLimit) { }
    }
    
    public class WithdrawAmountOutOfRangeException : DomainException
    {
        public WithdrawAmountOutOfRangeException() : base((int)ErrorCode.WithdrawAmountOutOfRange) { }
    }
}
