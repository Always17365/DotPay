using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Actor.Interfaces;
using Orleans;
using Dotpay.Common;

namespace Dotpay.Actor.Service.Interfaces
{
    public interface ITransferTransactionManager : Orleans.IGrainWithIntegerKey
    {
        Task Start();
        Task<ErrorCode> SubmitTransferTransaction(TransferTransactionInfo transactionInfo);
        Task<ErrorCode> MarkAsProcessing(Guid transferTransactionId, Guid operatorId);
        Task<ErrorCode> ConfirmTransactionFail(Guid transferTransactionId, Guid operatorId, string reason); 
        Task Receive(MqMessage message);
    }
}
