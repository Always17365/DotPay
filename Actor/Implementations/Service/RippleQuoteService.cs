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

        async Task<QuoteResult> IRippleQuoteService.Quote(Payway payway, string destionation, string realName, CurrencyType currency, decimal amount, string memo)
        {
            if (payway == Payway.Alipay || payway == Payway.Tenpay || payway == Payway.Bank)
            {
                var errorCode = await CheckToFiAmountRange(amount);
                if (errorCode != ErrorCode.None) return new QuoteResult(errorCode);
                var autoIncrement = GrainFactory.GetGrain<IAtomicIncrement>(AUTO_INCREMENT_KEY_FI);
                var rtfiId = await autoIncrement.GetNext();
                var destinationTag = Convert.ToInt64(payway.ToString("D") + rtfiId);

                var signMessage = destionation + payway +
                                  realName + currency + amount + memo;

                var invoiceId = GenerateInvoiceId(signMessage);

                var rippleToFiQuote = GrainFactory.GetGrain<IRippleToFiQuote>(rtfiId);
                var fee = await CalcToFiTransferFee(amount);
                var sendAmount = amount + fee;
                await rippleToFiQuote.Initialize(invoiceId, payway, destionation, realName, amount, amount + fee, memo);
                return new QuoteResult(errorCode, new Quote(destinationTag, invoiceId, sendAmount));
            }

            return new QuoteResult(ErrorCode.RippleQuoteUnsupport); ;
        }

        async Task<QuoteResult> IRippleQuoteService.Quote(Guid userId, string realName, CurrencyType currency, decimal amount, string memo)
        {

            //转到Dotpay，按照自动充值处理 
            var user = GrainFactory.GetGrain<IUser>(userId);
            var accountId = await user.GetAccountId();
            var errorCode = await CheckToDotpayAmountRange(amount);
            if (errorCode != ErrorCode.None) return new QuoteResult(errorCode);
            var account = GrainFactory.GetGrain<IAccount>(accountId);
            var autoIncrement = GrainFactory.GetGrain<IAtomicIncrement>(AUTO_INCREMENT_KEY_DOTPAY);
            var rtDotpayId = await autoIncrement.GetNext();
            var destinationTag = Convert.ToInt64(Payway.Dotpay.ToString("D") + rtDotpayId);
            var signMessage = accountId.ToString() + destinationTag + Payway.Dotpay.ToString("D") + currency + amount;

            var invoiceId = GenerateInvoiceId(signMessage);
            var fee = await CalcToDotpayTransferFee(amount);
            var sendAmount = amount + fee;
            var rippleToDotpayQuote = GrainFactory.GetGrain<IRippleToDotpayQuote>(rtDotpayId);
            await rippleToDotpayQuote.Initialize(userId, invoiceId, currency, amount, amount + fee, memo);
            return new QuoteResult(errorCode, new Quote(destinationTag, invoiceId, sendAmount));

        }

        private static string GenerateInvoiceId(string signMessage)
        {
            return Utilities.Sha256Sign(signMessage);
        }
        private static async Task<ErrorCode> CheckToFiAmountRange(decimal amount)
        {
            var setting = await GrainFactory.GetGrain<ISystemSetting>(0).GetRippleToFiSetting();

            var errorCode = (setting.MinAmount > amount || setting.MaxAmount < amount)
                            ? ErrorCode.RippleQuoteAmountOutOfRange
                            : ErrorCode.None;

            return errorCode;
        }
        private static async Task<decimal> CalcToFiTransferFee(decimal amount)
        {
            var fee = 0M;
            var setting = await GrainFactory.GetGrain<ISystemSetting>(0).GetRippleToFiSetting();

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
        private static async Task<decimal> CalcToDotpayTransferFee(decimal amount)
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
