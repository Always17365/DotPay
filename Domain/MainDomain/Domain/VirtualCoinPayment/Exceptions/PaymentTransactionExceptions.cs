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
    public class PaymentTransactionIsCompletedException : DomainException
    {
        public PaymentTransactionIsCompletedException() : base((int)ErrorCode.PaymentTransactionIsCompleted) { }
    }
    public class PaymentTransactionAmountError : DomainException
    {
        public PaymentTransactionAmountError() : base((int)ErrorCode.PaymentTransactionAmountError) { }
    }
}
