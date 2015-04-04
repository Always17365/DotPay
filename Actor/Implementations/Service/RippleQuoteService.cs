using System;
﻿using System.Threading.Tasks;
﻿using Dotpay.Actor;
using Dotpay.Actor.Ripple.Interfaces;
using Dotpay.Actor.Service;
using Dotpay.Actor.Tools.Interfaces;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class RippleQuoteService : Grain, IRippleQuoteService
    {
        private const string AutoIncrementKey = "RippleToFinancialInstitutionId";

        async Task<QuoteResult> IRippleQuoteService.Quote(TransferTargetInfo transferTargetInfo, CurrencyType currency, decimal amount, string memo)
        {

            var toFI = transferTargetInfo as TransferToFinancialInstitutionTargetInfo;

            if (toFI != null)
            {
                var errorCode = await CheckToFIAmountRange(amount);
                if (errorCode != ErrorCode.None) return new QuoteResult(errorCode);
                var autoIncrement = GrainFactory.GetGrain<IAtomicIncrement>(AutoIncrementKey);
                var rtfiId = await autoIncrement.GetNext();
                var destinationTag = Convert.ToInt64(toFI.Payway.ToString("D") + rtfiId);

                var signMessage = toFI.DestinationAccount + toFI.Payway +
                                  toFI.RealName + currency + amount + memo;

                var invoiceId = GenerateInvoiceId(signMessage);

                var rippleToFinancialInstitution = GrainFactory.GetGrain<IRippleToFinancialInstitution>(rtfiId);
                var fee = await CalcToFITransferFee(transferTargetInfo, amount);
                var sendAmount = amount + fee;
                await rippleToFinancialInstitution.Initialize(invoiceId, toFI, amount, amount + fee, memo);
                return new QuoteResult(errorCode, new Quote(destinationTag, invoiceId, sendAmount));
            }

            //转到Dotpay，按照自动充值处理
            var toDotpay = transferTargetInfo as TransferToDotpayTargetInfo;
            if (toDotpay != null)
            {
                var errorCode = await CheckToDotpayAmountRange(amount);
                if (errorCode != ErrorCode.None) return new QuoteResult(errorCode);
                var account = GrainFactory.GetGrain<IAccount>(toDotpay.AccountId);
                var userId = await account.GetOwnerId();
                var destinationTag = Convert.ToInt64(toDotpay.Payway.ToString("D") + userId);
                var signMessage = toDotpay.AccountId.ToString() + toDotpay.Payway + currency + amount;

                var invoiceId = GenerateInvoiceId(signMessage);
                var fee = await CalcToDotpayTransferFee(transferTargetInfo, amount);
                //应该做单独的对点付转账手续费设置
                var sendAmount = amount + fee;
                return new QuoteResult(errorCode, new Quote(destinationTag, invoiceId, sendAmount));
            }

            return new QuoteResult(ErrorCode.RippleQuoteUnsupport); ;
        }

        private static string GenerateInvoiceId(string signMessage)
        {
            return Utilities.Sha256Sign(signMessage);
        }
        private static async Task<ErrorCode> CheckToFIAmountRange(decimal amount)
        {
            var setting = await GrainFactory.GetGrain<ISystemSetting>(0).GetRippleToFinancialInstitutionSetting();

            var errorCode = (setting.MinAmount > amount || setting.MaxAmount < amount)
                            ? ErrorCode.RippleQuoteAmountOutOfRange
                            : ErrorCode.None;

            return errorCode;
        }
        private static async Task<decimal> CalcToFITransferFee(TransferTargetInfo transferTargetInfo, decimal amount)
        {
            var fee = 0M;
            var setting = await GrainFactory.GetGrain<ISystemSetting>(0).GetRippleToFinancialInstitutionSetting();

            fee += setting.FixedFee;
            fee += amount * setting.FeeRate;
            fee = Math.Ceiling(fee);
            fee = Math.Max(setting.MinFee, fee);
            fee = Math.Min(setting.MaxFee, fee);

            return fee;
        }

        private static async Task<ErrorCode> CheckToDotpayAmountRange(decimal amount)
        {
            var setting = await GrainFactory.GetGrain<ISystemSetting>(0).GetRippleToDotpaySetting();

            var errorCode = (setting.MinAmount > amount || setting.MaxAmount < amount)
                            ? ErrorCode.RippleQuoteAmountOutOfRange
                            : ErrorCode.None;

            return errorCode;
        }
        private static async Task<decimal> CalcToDotpayTransferFee(TransferTargetInfo transferTargetInfo, decimal amount)
        {
            var fee = 0M;
            var setting = await GrainFactory.GetGrain<ISystemSetting>(0).GetRippleToDotpaySetting();

            fee += setting.FixedFee;
            fee += amount * setting.FeeRate;
            fee = Math.Ceiling(fee);
            fee = Math.Max(setting.MinFee, fee);
            fee = Math.Min(setting.MaxFee, fee);

            return fee;
        }
    }
}
