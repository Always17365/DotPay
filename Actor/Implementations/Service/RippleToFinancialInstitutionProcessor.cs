 
﻿using System;
﻿using System.Collections.Concurrent;
﻿using System.Diagnostics;
﻿using System.Text;
﻿using System.Threading;
﻿using System.Threading.Tasks;
﻿using Dotpay.Actor.Interfaces;
﻿using Dotpay.Actor.Interfaces.Ripple;
﻿using Dotpay.Actor.Ripple.Interfaces;
﻿using Dotpay.Actor.Implementations;
﻿using Newtonsoft.Json;
﻿using Orleans;
﻿using Orleans.Concurrency;
﻿using Orleans.Runtime;
﻿using RabbitMQ.Client;
using DFramework;
﻿using Dotpay.Actor.Service.Interfaces;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class RippleToFinancialInstitutionProcessor : Grain, IRippleToFinancialInstitutionProcessor
    {
        private const string DepositTxManagerExchangeName = Constants.DepositTransactionManagerMQName + Constants.ExechangeSuffix;
        public async Task Receive(MqMessage message)
        {
            var rippleTxMessage = message as RippleToFITxMessage;

            if (rippleTxMessage != null)
            {
                //DestinationTag的前两位，是转账到哪里的标志位
                var destinationTag = rippleTxMessage.DestinationTag;

                var paywayFlg = destinationTag.ToString().Substring(0, 2);
                var identityFlg = Convert.ToInt64(destinationTag.ToString().Substring(2));
                Payway payway;
                Payway.TryParse(paywayFlg, true, out payway);

                if (payway == Payway.Dotpay)
                {
                    //此处要做幂等处理,直接发送充值消息
                    var user = GrainFactory.GetGrain<IUser>(identityFlg);
                    var depositTxId = Guid.NewGuid();
                    var accountId = await user.GetAccountId();
                    Debug.Assert(accountId.HasValue, "RippleToFinancialInstitutionProcessor accountId is null");

                    if (!ValidateInvoinceId(accountId.Value, payway, rippleTxMessage.Currency, rippleTxMessage.Amount,
                             rippleTxMessage.InvoiceId))
                    {
                        Log.Error("RippleToFinancialInstitutionProcessor InvoiceId Invalid.txid=" + rippleTxMessage.TxId);
                    }


                    var rippleDepositMessage = new RippleDepositTransactionMessage(depositTxId, accountId.Value,
                        rippleTxMessage.TxId, rippleTxMessage.Currency, rippleTxMessage.Amount, payway,
                        string.Empty);

                    await MessageProducterManager.GetProducter()
                         .PublishMessage(rippleDepositMessage, DepositTxManagerExchangeName, "", true);
                }
                else
                {
                    var rippleToFinancialInstitution = GrainFactory.GetGrain<IRippleToFinancialInstitution>(identityFlg);
                    await rippleToFinancialInstitution.Complete(rippleTxMessage.InvoiceId, rippleTxMessage.TxId, rippleTxMessage.Amount);
                }
            }
        }

        private bool ValidateInvoinceId(Guid accountId, Payway payway, CurrencyType currency, decimal amount, string invoiceId)
        {
            var signMessage = accountId.ToString() + payway + currency + amount;

            var _invoiceId = GenerateInvoiceId(signMessage);

            return invoiceId.Equals(_invoiceId, StringComparison.OrdinalIgnoreCase);
        }

        private static string GenerateInvoiceId(string signMessage)
        {
            return Utilities.Sha256Sign(signMessage);
        }
    }

}
