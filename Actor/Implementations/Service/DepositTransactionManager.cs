using System;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor.Implementations;
using Dotpay.Actor.Tools;
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

        async Task<ErrorCode> IDepositTransactionManager.ConfirmDepositTransaction(Guid depositTxId, Guid managerId, string transactionNo)
        {
            if (!await CheckManagerPermission(managerId)) return ErrorCode.HasNoPermission;

            var message = new ConfirmDepositTransactionMessage(depositTxId, managerId, transactionNo);

            await MessageProducterManager.GetProducter().PublishMessage(message, MqExchangeName, "", true);
            await ProcessConfirmDepositTransaction(message);

            return ErrorCode.None;
        }

        async Task<ErrorCode> IDepositTransactionManager.DepositTransactionMarkAsFail(Guid depositTxId, Guid managerId, string reason)
        {
            if (!await CheckManagerPermission(managerId)) return ErrorCode.HasNoPermission;

            //try
            //{
            var depositTx = GrainFactory.GetGrain<IDepositTransaction>(depositTxId);
            var depositTxState = await depositTx.GetStatus();

            if (depositTxState == DepositStatus.Started)
            {
                await depositTx.Fail(managerId, reason);
            }
            //}
            //catch (Exception ex)
            //{
            //    this.GetLogger().Error((int)ErrorCode.DepositTransactionManagerError, ex.Message, ex);
            //} 

            return ErrorCode.None;
        }

        async Task IDepositTransactionManager.Receive(MqMessage message)
        {
            var confirmMessage = message as ConfirmDepositTransactionMessage;

            if (confirmMessage != null)
                await ProcessConfirmDepositTransaction(confirmMessage);

            var rippleDeposit = message as RippleDepositTransactionMessage;
            if (rippleDeposit != null)
            {
                await ProcessRippleDepositTransaction(rippleDeposit);
            }
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
        private async Task ProcessRippleDepositTransaction(RippleDepositTransactionMessage rippleDepositMessage)
        {
            try
            {
                var depositTx = GrainFactory.GetGrain<IDepositTransaction>(rippleDepositMessage.DepositTxId);
                var rippleToDotpayQuote = GrainFactory.GetGrain<IRippleToDotpayQuote>(rippleDepositMessage.RippleToDotpayQuoteId);

                var errorCode = await rippleToDotpayQuote.Complete(rippleDepositMessage.InvoiceId, rippleDepositMessage.RippleTxId,
                        rippleDepositMessage.SendAmount);

                if (errorCode == ErrorCode.None)
                {
                    var rippleToDotpayQuoteInfo = await rippleToDotpayQuote.GetQuoteInfo();

                    var depositTxInfo = await depositTx.GetTransactionInfo();
                    var account = GrainFactory.GetGrain<IAccount>(depositTxInfo.AccountId);


                    if (await depositTx.GetStatus() < DepositStatus.Started)
                    {
                        var depositSequenceNo = await GeneratorDepositTransactionSequenceNo(Payway.Ripple);
                        await
                            depositTx.Initiliaze(depositSequenceNo, rippleDepositMessage.AccountId,
                                rippleToDotpayQuoteInfo.Currency, rippleToDotpayQuoteInfo.Amount, Payway.Ripple,
                                rippleToDotpayQuoteInfo.Memo);
                    }

                    if (await depositTx.GetStatus() == DepositStatus.Started)
                    {
                        await
                            account.AddTransactionPreparation(rippleDepositMessage.DepositTxId,
                                TransactionType.DepositTransaction,
                                PreparationType.CreditPreparation, depositTxInfo.Currency, depositTxInfo.Amount);
                        await depositTx.ConfirmDepositPreparation();
                    }

                    if (await depositTx.GetStatus() == DepositStatus.PreparationCompleted)
                    {
                        await account.CommitTransactionPreparation(rippleDepositMessage.DepositTxId);
                        await depositTx.ConfirmDeposit(null, rippleDepositMessage.RippleTxId);
                    }
                }
                else
                {
                    Log.Error("rippleToDotpayQuote.Complete Error:" + IoC.Resolve<IJsonSerializer>().Serialize(rippleDepositMessage));
                }
            }
            catch (Exception ex)
            {
                Log.Error("ProcessRippleDepositTransaction:" + ex.Message, ex);
                throw;
            }
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
                    await depositTx.ConfirmDeposit(confirmMessage.ManagerId, confirmMessage.TransactionNo);
                }
            }
            catch (Exception ex)
            {
                Log.Error("ProcessRippleDepositTransaction:" + ex.Message, ex);
                throw;
            }
        }

        private Task<bool> CheckManagerPermission(Guid operatorId)
        {
            var @operator = GrainFactory.GetGrain<IManager>(operatorId);

            return @operator.HasRole(ManagerType.DepositManager);
        }
        #endregion
    }
}
