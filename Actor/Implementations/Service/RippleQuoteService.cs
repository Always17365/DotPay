using System;
﻿using System.Threading.Tasks;
﻿using Dotpay.Actor.Interfaces;
﻿using Dotpay.Actor.Interfaces.Ripple;
using Dotpay.Actor.Interfaces.Tools;
using Dotpay.Actor.Service.Interfaces;
﻿using Dotpay.Common;
﻿using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor.Service.Implementations
{
    public class RippleQuoteService : Grain, IRippleQuoteService
    {
        private const string AutoIncrementKey = "RippleToFinancialInstitutionId";

        async Task<Immutable<QuoteResult>> IRippleQuoteService.Quote(Immutable<TransferTargetInfo> transferTargetInfo, decimal amount, string memo)
        {
            var errorCode = await CheckAmountRange(amount);

            if (errorCode != ErrorCode.None) return new QuoteResult(errorCode).AsImmutable();

            var autoIncrement = GrainFactory.GetGrain<IAtomicIncrement>(AutoIncrementKey);
            var rtfiId = await autoIncrement.GetNext();
            var signMessage = transferTargetInfo.Value.DestinationAccount + transferTargetInfo.Value.Payway +
                              transferTargetInfo.Value.RealName + amount + memo;

            var invoiceId = GenerateInvoiceId(signMessage);

            var rippleToFinancialInstitution = GrainFactory.GetGrain<IRippleToFinancialInstitution>(rtfiId);
            var fee = await CalcTransferFee(transferTargetInfo.Value, amount);
            var sendAmount = amount + fee;
            await rippleToFinancialInstitution.Initialize(invoiceId, transferTargetInfo, amount, amount + fee, memo);

            var result = new QuoteResult(errorCode, new Quote(rtfiId, invoiceId, sendAmount));

            return result.AsImmutable();
        }

        private static string GenerateInvoiceId(string signMessage)
        {
            return Utilities.Sha256Sign(signMessage);
        }
        private static async Task<ErrorCode> CheckAmountRange(decimal amount)
        {
            var setting = await GrainFactory.GetGrain<ISystemSetting>(0).GetRippleToFinancialInstitutionSetting();

            var errorCode = (setting.MinAmount > amount || setting.MaxAmount < amount)
                            ? ErrorCode.RippleQuoteAmountOutOfRange
                            : ErrorCode.None;

            return errorCode;
        }
        private static async Task<decimal> CalcTransferFee(TransferTargetInfo transferTargetInfo, decimal amount)
        {
            var fee = 0M;
            var setting = await GrainFactory.GetGrain<ISystemSetting>(0).GetRippleToFinancialInstitutionSetting();

            fee += setting.FixedFee;
            fee += amount * setting.FeeRate;
            fee = setting.MinFee.HasValue ? Math.Max(setting.MinFee.Value, fee) : fee;
            fee = setting.MaxFee.HasValue ? Math.Min(setting.MaxFee.Value, fee) : fee;

            return fee;
        }
    }
}
