using System;
using System.Threading.Tasks;
using Dotpay.Actor.Implementations;
using Dotpay.Actor.Interfaces;
using Dotpay.Actor.Service.Interfaces;
using Dotpay.Actor.Tools.Interfaces;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;
using RabbitMQ.Client;

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class DepositTransactionManager : Grain, IDepositTransactionManager
    {
        private const string MqExchangeName = Constants.DepositTransactionManagerMQName + Constants.ExechangeSuffix;
        private const string MqQueueName = Constants.DepositTransactionManagerMQName + Constants.QueueSuffix;

        async Task IDepositTransactionManager.CreateDepositTransaction(Guid depositTxId, Guid accountId, CurrencyType currency, decimal amount, Payway payway, string memo)
        {
            var depositTx = GrainFactory.GetGrain<IDepositTransaction>(depositTxId);
            var depositSequenceNo = await GeneratorDepositTransactionSequenceNo(payway);
            await depositTx.Initiliaze(depositSequenceNo, accountId, currency, amount, payway, memo);
        }

        async Task IDepositTransactionManager.ConfirmDepositTransaction(Guid depositTxId, Guid operatorId, string transactionNo)
        {
            var message = new ConfirmDepositTransactionMessage(depositTxId, operatorId, transactionNo);

            await MessageProducterManager.GetProducter().PublishMessage(message, MqExchangeName, durable: true);
            await ProcessConfirmDepositTransaction(message);
        }

        async Task IDepositTransactionManager.DepositTransactionMarkAsFail(Guid depositTxId, Guid operatorId, string reason)
        {
            //try
            //{
            var depositTx = GrainFactory.GetGrain<IDepositTransaction>(depositTxId);
            var depositTxState = await depositTx.GetStatus();

            if (depositTxState == DepositStatus.Started)
            {
                await depositTx.Fail(operatorId, reason);
            }
            //}
            //catch (Exception ex)
            //{
            //    this.GetLogger().Error((int)ErrorCode.DepositTransactionManagerError, ex.Message, ex);
            //}
        }

        async Task IDepositTransactionManager.Receive(MqMessage message)
        {
            var confirmMessage = message as ConfirmDepositTransactionMessage;

            if (confirmMessage != null)
                await ProcessConfirmDepositTransaction(confirmMessage);
        }

        public override Task OnActivateAsync()
        {
            MessageProducterManager.RegisterAndBindQueue(MqExchangeName, ExchangeType.Direct, MqQueueName, durable: true);
            return base.OnActivateAsync();
        }

        #region Private Methods
        private Task<string> GeneratorDepositTransactionSequenceNo(Payway payway)
        {
            //sequenceNoGeneratorId设计中只有5位,SequenceNoType占据3位 payway占据2位
            var sequenceNoGeneratorId = Convert.ToInt64(SequenceNoType.DepositTransaction.ToString() + payway.ToString());
            var sequenceNoGenerator = GrainFactory.GetGrain<ISequenceNoGenerator>(sequenceNoGeneratorId);

            return sequenceNoGenerator.GetNext();
        }
        private async Task ProcessConfirmDepositTransaction(ConfirmDepositTransactionMessage confirmMessage)
        {
            try
            {
                var depositTx = GrainFactory.GetGrain<IDepositTransaction>(confirmMessage.DepositTxId);
                var depositTxState = await depositTx.GetStatus();
                var depositTxInfo = await depositTx.GetTransactionInfo();
                var account = GrainFactory.GetGrain<IAccount>(depositTxInfo.AccountId);

                if (depositTxState == DepositStatus.Started)
                {
                    await account.AddTransactionPreparation(confirmMessage.DepositTxId, TransactionType.DepositTransaction,
                        PreparationType.CreditPreparation, depositTxInfo.Currency, depositTxInfo.Amount);
                    await depositTx.ConfirmDepositPreparation();
                }

                depositTxState = await depositTx.GetStatus();
                if (depositTxState == DepositStatus.PreparationCompleted)
                {
                    await account.CommitTransactionPreparation(confirmMessage.DepositTxId);
                    await depositTx.ConfirmDeposit(confirmMessage.OperatorId, confirmMessage.TransactionNo);
                }
            }
            catch (Exception ex)
            {
                this.GetLogger("ProcessConfirmDepositTransaction").Error((int)ErrorCode.DepositTransactionManagerError, ex.Message, ex);
            }
        }
        #endregion
    }
}
