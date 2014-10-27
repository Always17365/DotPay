using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.MainDomain.Events;

namespace DotPay.MainDomain
{
    public class FundSourceIsVerifiedException : DomainException
    {
        public FundSourceIsVerifiedException() : base((int)ErrorCode.FundSourceIsVerified) { }
    }

    public class FundSourceIsNotVerifiedException : DomainException
    {
        public FundSourceIsNotVerifiedException() : base((int)ErrorCode.FundSourceIsNotVerified) { }
    }

    public class FundSourceIsNotProcessingException : DomainException
    {
        public FundSourceIsNotProcessingException() : base((int)ErrorCode.FundSourceIsNotProcessing) { }
    }        

    public class FundSourceIsProcessingException : DomainException
    {
        public FundSourceIsProcessingException() : base((int)ErrorCode.FundSourceIsProcessing) { }
    }

    public class FundSourceIsNotCompletedException : DomainException
    {
        public FundSourceIsNotCompletedException() : base((int)ErrorCode.FundSourceIsNotCompleted) { }
    }

    public class FundSourceIsCompletedException : DomainException
    {
        public FundSourceIsCompletedException() : base((int)ErrorCode.FundSourceIsCompleted) { }
    }
    public class FundSourceIsDeletedException : DomainException
    {
        public FundSourceIsDeletedException() : base((int)ErrorCode.FundSourceIsDeleted) { }
    }
}
