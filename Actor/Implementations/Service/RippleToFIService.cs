 
﻿using System;
﻿using System.Diagnostics;
﻿using System.Threading.Tasks;
﻿using DFramework;
﻿using Dotpay.Actor.Implementations;
﻿using Dotpay.Actor.Ripple;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class RippleToFIService : Grain, IRippleToFiService
    {
        private const string DEPOSIT_TX_MANAGER_EXCHANGE_NAME = Constants.DepositTransactionManagerMQName + Constants.ExechangeSuffix;

        async Task<ErrorCode> IRippleToFiService.MarkAsProcessing(long rippleToFiTransactionId, Guid managerId)
        {
            if (!await this.CheckManagerPermission(managerId)) return ErrorCode.HasNoPermission;

            var rippleToFiTx = GrainFactory.GetGrain<IRippleToFiTransaction>(rippleToFiTransactionId);
            return await rippleToFiTx.Lock(managerId);
        }

        async Task<ErrorCode> IRippleToFiService.ConfirmTransactionComplete(long rippleToFiTransactionId, string transferNo, Guid managerId, string managerMemo)
        {
            if (!await this.CheckManagerPermission(managerId)) return ErrorCode.HasNoPermission;

            var rippleToFiTx = GrainFactory.GetGrain<IRippleToFiTransaction>(rippleToFiTransactionId);
            return await rippleToFiTx.Complete(transferNo, managerId, managerMemo);
        }

        async Task<ErrorCode> IRippleToFiService.ConfirmTransactionFail(long rippleToFiTransactionId, string reason, Guid managerId)
        {
            if (!await this.CheckManagerPermission(managerId)) return ErrorCode.HasNoPermission;

            var rippleToFiTx = GrainFactory.GetGrain<IRippleToFiTransaction>(rippleToFiTransactionId);
            return await rippleToFiTx.Fail(reason, managerId);
        }

        public async Task Receive(MqMessage message)
        {
            var rippleToFiTxMessage = message as RippleToFiTxMessage;

            if (rippleToFiTxMessage != null)
            {
                var rippleToFiQuoteId = rippleToFiTxMessage.RippleToFiTxId;
                var rippleToFiQuote = GrainFactory.GetGrain<IRippleToFiQuote>(rippleToFiQuoteId);
                var rippleToFiTx = GrainFactory.GetGrain<IRippleToFiTransaction>(rippleToFiTxMessage.RippleToFiTxId);

                var errorCode = await rippleToFiQuote.Complete(rippleToFiTxMessage.InvoiceId, rippleToFiTxMessage.TxId,
                      rippleToFiTxMessage.Amount);

                if (errorCode == ErrorCode.None)
                {
                    var rippleToFiQuoteInfo = await rippleToFiQuote.GetQuoteInfo();
                    await rippleToFiTx.Initialize(rippleToFiTxMessage.TxId, rippleToFiTxMessage.InvoiceId,
                                  rippleToFiQuoteInfo.Payway, rippleToFiQuoteInfo.Destination, rippleToFiQuoteInfo.RealName,
                                  rippleToFiQuoteInfo.Currency,rippleToFiQuoteInfo.Amount,
                                  rippleToFiQuoteInfo.SendAmount, rippleToFiQuoteInfo.Memo);
                }
            }
        }

        private Task<bool> CheckManagerPermission(Guid operatorId)
        {
            var @operator = GrainFactory.GetGrain<IManager>(operatorId);

            return @operator.HasRole(ManagerType.ToFiTransferManager);
        }
    }

}
