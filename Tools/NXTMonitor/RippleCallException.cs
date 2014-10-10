using System;

namespace DotPay.Tools.RippleClient
{
    class RippleCallException : Exception
    {
        public CallError Error
        {
            get;
            private set;
        }

        public RippleCallException(CallError callError)
            : base (callError.errorDescription)
        {
            Error = callError;
        }

        public RippleCallException(CallError callError, Exception innerException)
            : base(callError.errorDescription, innerException)
        {
            Error = callError;
        }
    }
}
