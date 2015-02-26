using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Actor.Interfaces; 
﻿using Dotpay.Actor.Service.Interfaces;
using Dotpay.Common;
using Dotpay.Common.Enum;
﻿using Orleans;

namespace Dotpay.Actors.Implementations.Service
{
    public class DepositTransactionManager : Orleans.Grain, IDepositTransactionManager
    {
        private const string MqExchangeName = "__DepositTransactionManagerExchange";
        private const string MqQueueName = "__DepositTransactionManagerQueue";
        private bool _started;
        internal static TaskScheduler OrleansScheduler;
        Task IDepositTransactionManager.CreateDepositTransaction(Guid depositTxId, Guid userId, CurrencyType currency, decimal amount, Payway payway, string memo)
        {
            var depositTx = GrainFactory.GetGrain<IDepositTransaction>(depositTxId);

            return depositTx.Initiliaze(userId, currency, amount, payway, memo);
        }

        async Task IDepositTransactionManager.ConfirmDepositTransaction(Guid depositTxId, Guid operatorId, string transactionNo)
        { 
            try
            {
                var depositTx = GrainFactory.GetGrain<IDepositTransaction>(depositTxId);
                var depositTxState = (await depositTx.GetStatus()).Value;
                var depositTxInfo = (await depositTx.GetTransactionInfo()).Value;
                var account = GrainFactory.GetGrain<IAccount>(depositTxInfo.AccountId);

                if (depositTxState == DepositStatus.Started)
                {
                    await account.AddTransactionPreparation(depositTxId, TransactionType.DepositTransaction,
                        PreparationType.CreditPreparation, depositTxInfo.Currency, depositTxInfo.Amount);
                }

                depositTxState = (await depositTx.GetStatus()).Value;
                if (depositTxState == DepositStatus.PreparationCompleted)
                {
                    await account.CommitTransactionPreparation(depositTxId);
                    await depositTx.ConfirmDeposit(operatorId, transactionNo);
                }
            }
            catch (Exception ex)
            {
                this.GetLogger("DepositTransactionManager").Error((int)ErrorCode.DepositTransactionManagerError, ex.Message, ex);
            }
        }

        async Task IDepositTransactionManager.DepositTransactionMarkAsFail(Guid depositTxId, Guid operatorId, string reason)
        {
            try
            {
                var depositTx = GrainFactory.GetGrain<IDepositTransaction>(depositTxId);
                var depositTxState = (await depositTx.GetStatus()).Value;

                if (depositTxState == DepositStatus.Started)
                {
                    await depositTx.Fail(operatorId, reason);
                }
            }
            catch (Exception ex)
            {
                this.GetLogger("DepositTransactionManager").Error((int)ErrorCode.DepositTransactionManagerError, ex.Message, ex);
            }
        }

        #region Private Methods

        private async Task InternalConfirmDepositTransaction(Guid depositTxId, Guid operatorId, string transactionNo)
        {
            try
            {
                var depositTx = GrainFactory.GetGrain<IDepositTransaction>(depositTxId);
                var depositTxState = (await depositTx.GetStatus()).Value;
                var depositTxInfo = (await depositTx.GetTransactionInfo()).Value;
                var account = GrainFactory.GetGrain<IAccount>(depositTxInfo.AccountId);

                if (depositTxState == DepositStatus.Started)
                {
                    await account.AddTransactionPreparation(depositTxId, TransactionType.DepositTransaction,
                        PreparationType.CreditPreparation, depositTxInfo.Currency, depositTxInfo.Amount);
                }

                depositTxState = (await depositTx.GetStatus()).Value;
                if (depositTxState == DepositStatus.PreparationCompleted)
                {
                    await account.CommitTransactionPreparation(depositTxId);
                    await depositTx.ConfirmDeposit(operatorId, transactionNo);
                }
            }
            catch (Exception ex)
            {
                this.GetLogger("DepositTransactionManager").Error((int)ErrorCode.DepositTransactionManagerError, ex.Message, ex);
            }
        }

        #endregion
    }
}
