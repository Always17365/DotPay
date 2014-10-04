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
    public class PaymentTransactionIsCompletedException : DomainException
    {
        public PaymentTransactionIsCompletedException() : base((int)ErrorCode.PaymentTransactionIsCompleted) { }
    }
    public class PaymentTransactionAmountError : DomainException
    {
        public PaymentTransactionAmountError() : base((int)ErrorCode.PaymentTransactionAmountError) { }
    }
}
