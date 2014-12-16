using System;
using RippleRPC.Net.Model;

namespace RippleRPC.Net.Exceptions
{
    public class RippleRpcException : Exception
    {
        public RippleError Error {get; private set;}

        public RippleRpcException(RippleError rippleError): base(rippleError.Message)
        {
            Error = rippleError;
        }

        public RippleRpcException(RippleError rippleError, Exception innerException)
            : base(rippleError.Message, innerException)
        {
            Error = rippleError;
        }
    }
}
