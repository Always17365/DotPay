using System;
using System.Threading.Tasks;
using Dotpay.Actor.Tools;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class RippleQuoteService : Grain, IRippleQuoteService
    {
        private const string AUTO_INCREMENT_KEY_FI = "RippleToFIId";
        private const string AUTO_INCREMENT_KEY_DOTPAY = "RippleToDotpayId";

        async Task<QuoteResult> IRippleQuoteService.Quote(TransferTargetInfo transferTargetInfo, CurrencyType currency, decimal amount, string memo)
        {
            var toFi = transferTargetInfo as TransferToFITargetInfo;

            if (toFi != null)
            {
                var errorCode = await CheckToFiAmountRange(amount);
                if (errorCode != ErrorCode.None) return new QuoteResult(errorCode);
                var autoIncrement = GrainFactory.GetGrain<IAtomicIncrement>(AUTO_INCREMENT_KEY_FI);
                var rtfiId = await autoIncrement.GetNext();
                var destinationTag = Convert.ToInt64(toFi.Payway.ToString("D") + rtfiId);

                var signMessage = toFi.DestinationAccount + toFi.Payway +
                                  toFi.RealName + currency + amount + memo;

                var invoiceId = GenerateInvoiceId(signMessage);

                var rippleToFiQuote = GrainFactory.GetGrain<IRippleToFIQuote>(rtfiId);
                var fee = await CalcToFiTransferFee(transferTargetInfo, amount);
                var sendAmount = amount + fee;
                await rippleToFiQuote.Initialize(invoiceId, toFi, amount, amount + fee, memo);
                return new QuoteResult(errorCode, new Quote(destinationTag, invoiceId, sendAmount));
            }

            //转到Dotpay，按照自动充值处理
            var toDotpay = transferTargetInfo as TransferToDotpayTargetInfo;
            if (toDotpay != null)
            {
                var errorCode = await CheckToDotpayAmountRange(amount);
                if (errorCode != ErrorCode.None) return new QuoteResult(errorCode);
                var account = GrainFactory.GetGrain<IAccount>(toDotpay.AccountId);
                var autoIncrement = GrainFactory.GetGrain<IAtomicIncrement>(AUTO_INCREMENT_KEY_DOTPAY);
                var rtDotpayId = await autoIncrement.GetNext();
                var destinationTag = Convert.ToInt64(toDotpay.Payway.ToString("D") + rtDotpayId);
                var signMessage = toDotpay.AccountId.ToString() + destinationTag + toDotpay.Payway + currency + amount;

                var invoiceId = GenerateInvoiceId(signMessage);
                var fee = await CalcToDotpayTransferFee(transferTargetInfo, amount);
                var sendAmount = amount + fee;
                var rippleToDotpayQuote = GrainFactory.GetGrain<IRippleToDotpayQuote>(rtDotpayId);
                await rippleToDotpayQuote.Initialize(await account.GetOwnerId(), invoiceId, toDotpay, currency, amount, amount + fee, memo);
                return new QuoteResult(errorCode, new Quote(destinationTag, invoiceId, sendAmount));
            }

            return new QuoteResult(ErrorCode.RippleQuoteUnsupport); ;
        }

        private static string GenerateInvoiceId(string signMessage)
        {
            return Utilities.Sha256Sign(signMessage);
        }
        private static async Task<ErrorCode> CheckToFiAmountRange(decimal amount)
        {
            var setting = await GrainFactory.GetGrain<ISystemSetting>(0).GetRippleToFISetting();

            var errorCode = (setting.MinAmount > amount || setting.MaxAmount < amount)
                            ? ErrorCode.RippleQuoteAmountOutOfRange
                            : ErrorCode.None;

            return errorCode;
        }
        private static async Task<decimal> CalcToFiTransferFee(TransferTargetInfo transferTargetInfo, decimal amount)
        {
            var fee = 0M;
            var setting = await GrainFactory.GetGrain<ISystemSetting>(0).GetRippleToFISetting();

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
