using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Dotpay.Actor.Implementations;
using Dotpay.Actor;
using Dotpay.Actor.Service;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using RabbitMQ.Client;

namespace Dotpay.Actor.Service.Implementations
{
    public class RefundTransactionManager : Grain, IRefundTransactionManager
    {
        private const string MqExchangeName = Constants.RefundTransactionManagerMQName + Constants.ExechangeSuffix;
        private const string MqQueueName = Constants.RefundTransactionManagerMQName + Constants.QueueSuffix; 

        public async Task Receive(MqMessage message)
        {
            var refundMessage = message as RefundTransactionMessage;

            if (refundMessage != null)
            {
                if (refundMessage.RefundTransactionType == RefundTransactionType.TransferRefund)
                {
                    var transferTx = GrainFactory.GetGrain<ITransferTransaction>(refundMessage.TransactionId);
                    var transferTxInfo = await transferTx.GetTransactionInfo();
                    var refundTx = GrainFactory.GetGrain<IRefundTransaction>(refundMessage.RefundTransactionId);
                    var account = GrainFactory.GetGrain<IAccount>(transferTxInfo.Source.AccountId);

                    if (await refundTx.GetStatus() < RefundTransactionStatus.Initalized)
                    {
                        await refundTx.Initiliaze(refundMessage.TransactionId, transferTxInfo.Source.AccountId,
                            refundMessage.RefundTransactionType, transferTxInfo.Currency, transferTxInfo.Amount);
                    }

                    if (await refundTx.GetStatus() == RefundTransactionStatus.Initalized)
                    {
                        await account.AddTransactionPreparation(refundMessage.RefundTransactionId,
                                TransactionType.RefundTransaction, PreparationType.CreditPreparation,
                                transferTxInfo.Currency, transferTxInfo.Amount);
                        await refundTx.ConfirmRefundPreparation();
                    }

                    if (await refundTx.GetStatus() == RefundTransactionStatus.PreparationCompleted)
                    {
                        await account.CommitTransactionPreparation(refundMessage.RefundTransactionId);
                        await refundTx.Confirm();
                    }
                }
            }
        }

        #region Override
        public override async Task OnActivateAsync()
        {
            await MessageProducterManager.RegisterAndBindQueue(MqExchangeName, ExchangeType.Direct, MqQueueName, durable: true);
            await base.OnActivateAsync();
        }
        #endregion


    }
}
