﻿using System;
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
    public class TransferTransactionNotPendingException : DomainException
    {
        public TransferTransactionNotPendingException() : base((int)ErrorCode.TransferTransactionNotPending) { }
    }
}