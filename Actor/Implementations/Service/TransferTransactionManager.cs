using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor.Implementations;
using Dotpay.Actor.Tools;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using RabbitMQ.Client;

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class TransferTransactionManager : Grain, ITransferTransactionManager, IRemindable
    {
        private const string MqTransferExchangeName = Constants.TransferTransactionManagerMQName + Constants.ExechangeSuffix;
        private const string MqTransferQueueName = Constants.TransferTransactionManagerMQName + Constants.QueueSuffix;
        private const string MqTransferRouteKey = Constants.TransferTransactionManagerRouteKey;
        private const string MqToRippleQueueRouteKey = Constants.TransferToRippleRouteKey;
        private const string MqToRippleQueueName = Constants.TransferToRippleQueueName + Constants.QueueSuffix;
        private const string MqRefundExchangeName = Constants.RefundTransactionManagerMQName + Constants.ExechangeSuffix;

        private static readonly ConcurrentDictionary<Guid, TaskScheduler> OrleansSchedulerContainer =
            new ConcurrentDictionary<Guid, TaskScheduler>();


        async Task<ErrorCode> ITransferTransactionManager.SubmitTransferToDotpayTransaction(Guid transferTransactionId, Guid sourceAccountId, Guid targetAccountId, string targetUserRealName, CurrencyType currency, decimal amount, string memo, string paymentPassword)
        {
            var checkPaymentPasswordResult = await CheckUserPaymentPassword(sourceAccountId, paymentPassword);
            if (checkPaymentPasswordResult != ErrorCode.None)
                return checkPaymentPasswordResult;

            var transferTransactionInfo = await BuildTransferToDotpayTransactionInfo(sourceAccountId, targetAccountId,
                targetUserRealName, currency, amount, memo);
            return await SubmitTransferTransaction(transferTransactionId, transferTransactionInfo);
        }

        async Task<ErrorCode> ITransferTransactionManager.SubmitTransferToTppTransaction(Guid transferTransactionId, Guid sourceAccountId, string targetAccount, string realName, Payway targetPayway, CurrencyType currency, decimal amount, string memo, string paymentPassword)
        {
            var checkPaymentPasswordResult = await CheckUserPaymentPassword(sourceAccountId, paymentPassword);
            if (checkPaymentPasswordResult != ErrorCode.None)
                return checkPaymentPasswordResult;

            var transferTransactionInfo = await BuildTransferToTppTransactionInfo(sourceAccountId, targetAccount,
               realName, targetPayway, currency, amount, memo);
            return await SubmitTransferTransaction(transferTransactionId, transferTransactionInfo);
        }

        async Task<ErrorCode> ITransferTransactionManager.SubmitTransferToBankTransaction(Guid transferTransactionId, Guid sourceAccountId, string targetAccount, string realName, Bank targetBank, CurrencyType currency, decimal amount, string memo, string paymentPassword)
        {
            var checkPaymentPasswordResult = await CheckUserPaymentPassword(sourceAccountId, paymentPassword);
            if (checkPaymentPasswordResult != ErrorCode.None)
                return checkPaymentPasswordResult;

            var transferTransactionInfo = await BuildTransferToBankTransactionInfo(sourceAccountId, targetAccount,
                 realName, targetBank, currency, amount, memo);
            return await SubmitTransferTransaction(transferTransactionId, transferTransactionInfo);

        }

        async Task<ErrorCode> ITransferTransactionManager.SubmitTransferToRippleTransaction(Guid transferTransactionId, Guid sourceAccountId, string rippleAddress, CurrencyType currency, decimal amount, string memo, string paymentPassword)
        {
            var checkPaymentPasswordResult = await CheckUserPaymentPassword(sourceAccountId, paymentPassword);
            if (checkPaymentPasswordResult != ErrorCode.None)
                return checkPaymentPasswordResult;

            var transferTransactionInfo = await BuildTransferToRippleTransactionInfo(sourceAccountId,
                                          rippleAddress, currency, amount, memo);
            return await SubmitTransferTransaction(transferTransactionId, transferTransactionInfo);

        }

        async Task<ErrorCode> ITransferTransactionManager.MarkAsProcessing(Guid transferTransactionId, Guid managerId)
        {
            if (!await CheckManagerPermission(managerId)) return ErrorCode.HasNoPermission;

            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
            return await transferTransaction.MarkAsProcessing(managerId);
        }

        async Task<ErrorCode> ITransferTransactionManager.ConfirmTransactionFail(Guid transferTransactionId,
            Guid managerId, string reason)
        {
            if (!await CheckManagerPermission(managerId)) return ErrorCode.HasNoPermission;

            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
            var code = await transferTransaction.ConfirmFail(managerId, reason);
            if (code == ErrorCode.None)
                await PublishRefundMessage(transferTransactionId);

            return code;
        }

        async Task<ErrorCode> ITransferTransactionManager.ConfirmTransactionComplete(Guid transferTransactionId, Guid managerId, string transferNo)
        {
            if (!await CheckManagerPermission(managerId)) return ErrorCode.HasNoPermission;

            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
            var code = await transferTransaction.ConfirmComplete(managerId, transferNo);

            return code;
        }

        async Task ITransferTransactionManager.Receive(MqMessage message)
        {
            var transactionMessage = message as SubmitTransferTransactionMessage;

            if (transactionMessage != null)
            {
                await ProcessSubmitedTransferTransactionMessage(transactionMessage);
                return;
            }

            var rippleTransactionPresubmitMessage = message as RippleTransactionPresubmitMessage;

            if (rippleTransactionPresubmitMessage != null)
            {
                await ProcessRippleTransactionPresubmitMessage(rippleTransactionPresubmitMessage);
                return;
            }
        }

        #region override
        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            //Reminder用来做3分钟后检查RippleTx结果
            var transferTxId = new Guid(reminderName);
            var transferTx = GrainFactory.GetGrain<ITransferTransaction>(transferTxId);
            var rippleTxStatus = await transferTx.GetRippleTransactionStatus();

            if (rippleTxStatus == RippleTransactionStatus.Submited)
            {
                var rippleTxInfo = await transferTx.GetRippleTransactionInfo();
                var rippleRpcClient = GrainFactory.GetGrain<IRippleRpcClient>(0);
                var currentLedgerIndex = await rippleRpcClient.GetLastLedgerIndex();
                if (rippleTxInfo.LastLedgerIndex < currentLedgerIndex)
                {
                    var validateResult = await rippleRpcClient.ValidateRippleTx(rippleTxInfo.RippleTxId);

                    if (validateResult == RippleTransactionValidateResult.NotFound)
                    {
                        //resubmit
                        await transferTx.ReSubmitToRipple();
                        var transaferTxInfo = await transferTx.GetTransactionInfo();
                        await
                            PublishSubmitToRippleMessage(transferTxId, transaferTxInfo.Target.Destination,
                                transaferTxInfo.Currency, transaferTxInfo.Amount);
                    }
                    else if (validateResult == RippleTransactionValidateResult.Success)
                    {
                        //complete
                        await transferTx.RippleTransactionComplete(rippleTxInfo.RippleTxId);
                    }
                }
            }
        }


        public override async Task OnActivateAsync()
        {
            await RegisterAndBindMqQueue();
            await base.OnActivateAsync();
        }
        #endregion

        #region Private Methods

        #region BuildTransferTransactionInfo

        private async Task<TransferTransactionInfo> BuildTransferToDotpayTransactionInfo(Guid sourceAccountId, Guid targetAccountId,
           string targetUserRealName, CurrencyType currency, decimal amount, string memo)
        {
            var sourceAccount = GrainFactory.GetGrain<IAccount>(sourceAccountId);
            var sourceUserId = await sourceAccount.GetOwnerId();
            var sourceUser = GrainFactory.GetGrain<IUser>(sourceUserId);
            var sourceUserInfo = await sourceUser.GetUserInfo();
            var transferFromDotpayinfo = new TransferSourceInfo()
            {
                AccountId = sourceAccountId,
                Email = sourceUserInfo.Email,
                UserLoginName = sourceUserInfo.LoginName,
                Payway = Payway.Dotpay,
                UserId = sourceUserId
            };

            var targetAccount = GrainFactory.GetGrain<IAccount>(targetAccountId);
            var targetUserId = await targetAccount.GetOwnerId();
            var targetUser = GrainFactory.GetGrain<IUser>(targetUserId);
            var targetUserInfo = await targetUser.GetUserInfo();
            var transferToDotpayinfo = new TransferTargetInfo()
            {
                AccountId = targetAccountId,
                Email = targetUserInfo.Email,
                UserLoginName = targetUserInfo.LoginName,
                Payway = Payway.Dotpay,
                UserId = targetUserId,
                RealName = targetUserRealName
            };

            var transferTransactionInfo = new TransferTransactionInfo(transferFromDotpayinfo, transferToDotpayinfo,
                currency, amount, memo);


            return transferTransactionInfo;
        }
        private async Task<TransferTransactionInfo> BuildTransferToTppTransactionInfo(Guid sourceAccountId, string targetAccount, string realName,
            Payway targetPayway, CurrencyType currency, decimal amount, string memo)
        {
            var sourceAccount = GrainFactory.GetGrain<IAccount>(sourceAccountId);
            var sourceUserId = await sourceAccount.GetOwnerId();
            var sourceUser = GrainFactory.GetGrain<IUser>(sourceUserId);
            var sourceUserInfo = await sourceUser.GetUserInfo();
            var transferFromDotpayinfo = new TransferSourceInfo()
            {
                AccountId = sourceAccountId,
                Email = sourceUserInfo.Email,
                UserLoginName = sourceUserInfo.LoginName,
                Payway = Payway.Dotpay,
                UserId = sourceUserId
            };

            var transferToTppTargetInfo = new TransferTargetInfo()
            {
                Payway = targetPayway,
                Destination = targetAccount,
                RealName = realName
            };

            var transferTransactionInfo = new TransferTransactionInfo(transferFromDotpayinfo, transferToTppTargetInfo,
                currency, amount, memo);


            return transferTransactionInfo;
        }
        private async Task<TransferTransactionInfo> BuildTransferToBankTransactionInfo(Guid sourceAccountId, string targetAccount, string realName,
          Bank targetBank, CurrencyType currency, decimal amount, string memo)
        {
            var sourceAccount = GrainFactory.GetGrain<IAccount>(sourceAccountId);
            var sourceUserId = await sourceAccount.GetOwnerId();
            var sourceUser = GrainFactory.GetGrain<IUser>(sourceUserId);
            var sourceUserInfo = await sourceUser.GetUserInfo();
            var transferFromDotpayinfo = new TransferSourceInfo()
            {
                AccountId = sourceAccountId,
                Email = sourceUserInfo.Email,
                UserLoginName = sourceUserInfo.LoginName,
                Payway = Payway.Dotpay,
                UserId = sourceUserId
            };

            var transferToBankTargetInfo = new TransferTargetInfo()
            {
                Payway = Payway.Bank,
                Bank = targetBank,
                Destination = targetAccount,
                RealName = realName
            };

            var transferTransactionInfo = new TransferTransactionInfo(transferFromDotpayinfo, transferToBankTargetInfo,
                currency, amount, memo);


            return transferTransactionInfo;
        }
        private async Task<TransferTransactionInfo> BuildTransferToRippleTransactionInfo(Guid sourceAccountId, string rippleAddress,
            CurrencyType currency, decimal amount, string memo)
        {
            var sourceAccount = GrainFactory.GetGrain<IAccount>(sourceAccountId);
            var sourceUserId = await sourceAccount.GetOwnerId();
            var sourceUser = GrainFactory.GetGrain<IUser>(sourceUserId);
            var sourceUserInfo = await sourceUser.GetUserInfo();
            var transferFromDotpayinfo = new TransferSourceInfo()
            {
                AccountId = sourceAccountId,
                Email = sourceUserInfo.Email,
                UserLoginName = sourceUserInfo.LoginName,
                Payway = Payway.Dotpay,
                UserId = sourceUserId
            };

            var transferToBankTargetInfo = new TransferTargetInfo()
            {
                Payway = Payway.Ripple,
                Destination = rippleAddress
            };

            var transferTransactionInfo = new TransferTransactionInfo(transferFromDotpayinfo, transferToBankTargetInfo,
                currency, amount, memo);


            return transferTransactionInfo;
        }
        #endregion
        /// <summary>
        /// 根据用户的AccountId去检查用户的支付密码
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        private async Task<ErrorCode> CheckUserPaymentPassword(Guid accountId, string paymentPassword)
        {
            var account = GrainFactory.GetGrain<IAccount>(accountId);
            var userId = await account.GetOwnerId();

            var user = GrainFactory.GetGrain<IUser>(userId);
            return await user.CheckPaymentPassword(paymentPassword);
        }

        private async Task<ErrorCode> SubmitTransferTransaction(Guid transferTransactionId,
            TransferTransactionInfo transactionInfo)
        {
            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
            var transferTransactionSeq =
                await this.GeneratorTransferTransactionSequenceNo(transactionInfo.Target.Payway);
            await transferTransaction.Initialize(transferTransactionSeq, transactionInfo);

            var message = new SubmitTransferTransactionMessage(transferTransactionId, transactionInfo.Source.UserId,
                transactionInfo.Target, transactionInfo.Currency, transactionInfo.Amount, transactionInfo.Memo,
                TransferTransactionMessageType.SubmitTransferTransactionMessage);
            await MessageProducterManager.GetProducter()
                    .PublishMessage(message, MqTransferExchangeName, MqTransferRouteKey, true);
            return await ProcessSubmitedTransferTransactionMessage(message);
        }

        private Task<string> GeneratorTransferTransactionSequenceNo(Payway payway)
        {
            //sequenceNoGeneratorId设计中只有5位,SequenceNoType占据3位 payway占据2位
            var sequenceNoGeneratorId = Convert.ToInt64(SequenceNoType.TransferTransaction.ToString("D") + payway.ToString("D"));
            var sequenceNoGenerator = GrainFactory.GetGrain<ISequenceNoGenerator>(sequenceNoGeneratorId);

            return sequenceNoGenerator.GetNext();
        }

        private async Task<ErrorCode> ProcessSubmitedTransferTransactionMessage(SubmitTransferTransactionMessage message)
        {
            var user = GrainFactory.GetGrain<IUser>(message.UserId);
            var accountId = await user.GetAccountId();
            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(message.TransferTransactionId);
            var sourceAccount = GrainFactory.GetGrain<IAccount>(accountId);
            IAccount targetAccount = null;
            var sourceUserInfo = await user.GetUserInfo();

            var source = new TransferSourceInfo()
            {
                AccountId = accountId,
                UserId = message.UserId,
                UserLoginName = sourceUserInfo.LoginName,
                Payway = Payway.Dotpay
            };

            var target = message.Target.AsImmutable().GetCopy();
            //如果是点付内部转账，验证转入账户,并读取用户信息以便做冗余
            if (message.Target.Payway == Payway.Dotpay)
            {
                target = message.Target;
                targetAccount = GrainFactory.GetGrain<IAccount>(target.AccountId);

                var targetUserId = await targetAccount.GetOwnerId();
                var targetUserInfo = await GrainFactory.GetGrain<IUser>(targetUserId).GetUserInfo();
                target = new TransferTargetInfo()
                {
                    AccountId = target.AccountId,
                    Payway = target.Payway,
                    UserId = targetUserId,
                    RealName = target.RealName,
                    UserLoginName = targetUserInfo.LoginName
                };
            }

            if (await transferTransaction.GetStatus() == TransferTransactionStatus.Submited)
            {
                var validateTasks = new List<Task<bool>>();
                validateTasks.Add(sourceAccount.Validate());

                if (targetAccount != null) validateTasks.Add(targetAccount.Validate());

                var results = await Task.WhenAll(validateTasks);

                //如果转出账户有问题，则取消转账
                if (results[0] == false)
                {
                    await transferTransaction.Cancel(TransferTransactionCancelReason.SourceAccountNotExist);
                    return ErrorCode.TranasferTransactionSourceAccountNotExist;
                }
                else if (targetAccount != null && results[1] == false)
                {
                    await transferTransaction.Cancel(TransferTransactionCancelReason.TargetAccountNotExist);
                    return ErrorCode.TranasferTransactionTargetAccountNotExist;
                }

                var preaparationTasks = new List<Task<ErrorCode>>();
                preaparationTasks.Add(sourceAccount.AddTransactionPreparation(message.TransferTransactionId,
                     TransactionType.TransferTransaction, PreparationType.DebitPreparation, message.Currency, message.Amount));

                if (targetAccount != null)
                    preaparationTasks.Add(targetAccount.AddTransactionPreparation(message.TransferTransactionId,
                      TransactionType.TransferTransaction, PreparationType.CreditPreparation, message.Currency, message.Amount));


                var opCodeResults = await Task.WhenAll(preaparationTasks);
                //如果转出账户金额充足
                if (opCodeResults[0] == ErrorCode.AccountBalanceNotEnough)
                {
                    return ErrorCode.AccountBalanceNotEnough;
                    //await transferTransaction.Cancel(TransferTransactionCancelReason.BalanceNotEnough);
                    //await sourceAccount.CancelTransactionPreparation(message.TransferTransactionId);
                }
                await transferTransaction.ConfirmTransactionPreparation();
            }

            if (await transferTransaction.GetStatus() == TransferTransactionStatus.PreparationCompleted)
            {
                var commitTasks = new List<Task>();
                commitTasks.Add(sourceAccount.CommitTransactionPreparation(message.TransferTransactionId));
                if (targetAccount != null)
                    commitTasks.Add(targetAccount.CommitTransactionPreparation(message.TransferTransactionId));

                await Task.WhenAll(commitTasks);

                if (message.Target.Payway == Payway.Ripple)
                {
                    await transferTransaction.SubmitToRipple();
                    await PublishSubmitToRippleMessage(message.TransferTransactionId, target.Destination, message.Currency, message.Amount);
                }

            }

            return ErrorCode.None;
        }

        //处理ripple presubmit message
        private async Task ProcessRippleTransactionPresubmitMessage(RippleTransactionPresubmitMessage message)
        {
            var transferTransactionId = message.TransferTransactionId;
            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
            var txInfo = await transferTransaction.GetTransactionInfo();

            if (message.Currency != txInfo.Currency || txInfo.Amount != message.Amount)
            {
                var errorMsg =
                    "ProcessRippleTransactionPresubmitMessage Currency or Amount not match! tx currency={0},message currency={1}; tx amount={2}, message amount={3}"
                        .FormatWith(txInfo.Currency, message.Currency, txInfo.Amount, message.Amount);
                this.GetLogger().Error(-1, errorMsg);

                Debug.Assert((message.Currency == txInfo.Currency && txInfo.Amount != message.Amount), errorMsg);
            }

            await transferTransaction.RippleTransactionPersubmit(message.RippleTxId, message.LastLedgerIndex);
            //如果遇到一个tx，多次presubmit,每次调用presubmit都会取消上次的Reminder,最后一次的Presubmit Reminder才会生效
            await
                this.RegisterOrUpdateReminder(message.TransferTransactionId.ToString(), TimeSpan.FromMinutes(3),
                    TimeSpan.FromMinutes(2));
        }

        //处理ripple presubmit message
        private async Task ProcessRippleTransactionResultMessage(RippleTransactionResultMessage message)
        {
            var transferTransactionId = message.TransferTransactionId;
            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
            var txInfo = await transferTransaction.GetTransactionInfo();

            if (message.Success)
                await transferTransaction.RippleTransactionComplete(message.RippleTxId);
            else
            {
                await transferTransaction.RippleTransactionFail(message.RippleTxId, message.FailedReason);
                await PublishRefundMessage(transferTransactionId);
            }
        }

        private async Task RegisterAndBindMqQueue()
        {
            await MessageProducterManager.RegisterAndBindQueue(MqTransferExchangeName, ExchangeType.Direct, MqTransferQueueName,
                  MqTransferRouteKey, true);
            await MessageProducterManager.RegisterAndBindQueue(MqTransferExchangeName, ExchangeType.Direct, MqToRippleQueueName,
                  MqToRippleQueueRouteKey, true);
        }

        private async Task PublishSubmitToRippleMessage(Guid transferTransactionId, string destination,
            CurrencyType currency, decimal amount)
        {
            var toRippleMessage = new SubmitTransferTransactionToRippleMessage(transferTransactionId,
                destination, currency, amount);
            await MessageProducterManager.GetProducter()
                .PublishMessage(toRippleMessage, MqTransferExchangeName, MqToRippleQueueRouteKey, true);
        }

        private async Task PublishRefundMessage(Guid transferTransactionId)
        {
            var toRippleMessage = new RefundTransactionMessage(Guid.NewGuid(), transferTransactionId,
                RefundTransactionType.TransferRefund);
            await MessageProducterManager.GetProducter()
                .PublishMessage(toRippleMessage, MqRefundExchangeName, "", true);
        }
        private Task<bool> CheckManagerPermission(Guid operatorId)
        {
            var @operator = GrainFactory.GetGrain<IManager>(operatorId);

            return @operator.HasRole(ManagerType.TransferManager);
        }
        #endregion
    }
}