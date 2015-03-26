using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Actor.Interfaces;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor.Service.Interfaces
{
    public interface IRefundTransactionManager : Orleans.IGrainWithIntegerKey
    {
        Task Receive(MqMessage message);
    }

    [Immutable]
    [Serializable]
    public class RefundTransactionMessage : MqMessage
    {
        public RefundTransactionMessage(Guid refundTransactionId, Guid transactionId, RefundTransactionType refundTransactionType)
        {
            this.RefundTransactionId = refundTransactionId;
            this.TransactionId = transactionId;
            this.RefundTransactionType = refundTransactionType;
        }

        public Guid RefundTransactionId { get; private set; }
        public Guid TransactionId { get; private set; }
        public RefundTransactionType RefundTransactionType { get; private set; }
    }
}
