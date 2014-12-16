using FC.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;

namespace DotPay.Common
{
    public class Utilities
    {
        #region message queue

        /// <summary>
        /// 获取用户注册成功的消息exchange and queue 名字
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary> 
        /// <returns></returns>
        public static Tuple<string, string> GetExchangeAndQueueNameOfUserRegistered()
        {
            var exchangeName = "_USER_REGISTER_EXCHANGE";
            var queueName = "_USER_REGISTER_QUEUE";

            return new Tuple<string, string>(exchangeName, queueName);
        }

        /// <summary>
        /// generate exchange and queue name for inbound transafer
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary> 
        /// <returns></returns>
        public static Tuple<string, string> GenerateExchangeAndQueueNameOfInboundTransfer()
        {
            var exchangeName = "_INBOUND_TRANSFER_EXCHANGE";
            var queueName = "_INBOUND_TRANSFER_QUEUE";

            return new Tuple<string, string>(exchangeName, queueName);
        }


        /// <summary>
        /// generate exchange and queue name for outbound transafer
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary> 
        /// <returns></returns>
        public static Tuple<string, string> GenerateExchangeAndQueueNameOfOutboundTransferForSign()
        {
            var exchangeName = "_OUTBOUND_TRANSFER_EXCHANGE_SIGN";
            var queueName = "_OUTBOUND_TRANSFER_QUEUE_SIGN";  

            return new Tuple<string, string>(exchangeName, queueName);
        }

        /// <summary>
        /// generate exchange and queue name for outbound transafer
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary> 
        /// <returns></returns>
        public static Tuple<string, string> GenerateExchangeAndQueueNameOfOutboundTransferForSubmit()
        {
            var exchangeName = "_OUTBOUND_TRANSFER_EXCHANGE_SUBMIT";
            var queueName = "_OUTBOUND_TRANSFER_QUEUE_SUBMIT";

            return new Tuple<string, string>(exchangeName, queueName);
        }

        /// <summary>
        /// generate exchange and queue name for outbound transafer process complete
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary> 
        /// <returns></returns>
        public static Tuple<string, string> GenerateExchangeAndQueueNameOfOutboundTransferProcessComplete()
        {
            var exchangeName = "_OUTBOUND_TRANSFER_EXCHANGE_COMPLETE";
            var queueName = "_OUTBOUND_TRANSFER_QUEUE_COMPLETE";

            return new Tuple<string, string>(exchangeName, queueName);
        }
        /// <summary>
        /// generate virtual coin reveive payment exchange and queue name
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary>
        /// <param name="CurrencyType">currency</param>
        /// <returns></returns>
        public static Tuple<string, string> GenerateVirtualCoinReceivePaymentExchangeAndQueueName(CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            var exchangeName = currency.ToString() + "_GENERATE_RECEIVE_PAYMENT_EXCHANGE";
            var queueName = currency.ToString() + "_GENERATE_RECEIVE_PAYMENT_QUEUE";

            return new Tuple<string, string>(exchangeName, queueName);
        }

        /// <summary>
        /// generate virtual coin deposit exchange and queue name
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary>
        /// <param name="CurrencyType">currency</param>
        /// <returns></returns>
        public static Tuple<string, string> GenerateVirtualCoinDepositExchangeAndQueueName()
        {
            var exchangeName = "__VIRTUAL_COIN_EXCHANGE";
            var queueName = "__VIRTUAL_COIN_QUEUE";

            return new Tuple<string, string>(exchangeName, queueName);
        }


        /// <summary>
        /// generate virtual coin send payment exchange and queue name
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary>
        /// <param name="CurrencyType">currency</param>
        ///<returns></returns>
        public static Tuple<string, string> GenerateVirtualCoinSendPaymentExchangeAndQueueName(CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            var exchangeName = currency.ToString() + "_GENERATE_SEND_PAYMENT_EXCHANGE";
            var queueName = currency.ToString() + "_GENERATE_SEND_PAYMENT_QUEUE";

            return new Tuple<string, string>(exchangeName, queueName);
        }

        /// <summary>
        /// generate virtual coin complete payment exchange and queue name
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary>
        ///<returns></returns>
        public static Tuple<string, string> GenerateVirtualCoinCompletePaymentExchangeAndQueueName()
        {
            var exchangeName = "___GENERATE_COMPLETE_PAYMENT_EXCHANGE";
            var queueName = "___GENERATE_COMPLETE_PAYMENT_QUEUE";

            return new Tuple<string, string>(exchangeName, queueName);
        }

        /// <summary>
        /// generate virtual coin tranfer fail or address invalid exchange and queue name
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary> 
        ///<returns></returns>
        public static Tuple<string, string> GenerateVirtualCoinWithdrawTranferFailOrAddressInvalidExchangeAndQueueName()
        {
            var exchangeName = "_VirtualCoinWithdrawTranferFailOrAddressInvalid_EXCHANGE";
            var queueName = "_VirtualCoinWithdrawTranferFailOrAddressInvalid_QUEUE";

            return new Tuple<string, string>(exchangeName, queueName);
        }

        /// <summary>
        /// generate virtual coin balance warn message exchange and queue name
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary> 
        ///<returns></returns>
        public static Tuple<string, string> GenerateBanlanceWarnExchangeAndQueueName()
        {
            var exchangeName = "_VIRTUALCOIN_BALANCE_WARN_MESSAGE_EXCHANGE";
            var queueName = "_VIRTUALCOIN_BALANCE_WARN_MESSAGE_QUEUE";

            return new Tuple<string, string>(exchangeName, queueName);
        }
        #endregion


        public static PayWay GetPayway(string bridge)
        {
            var payway = default(PayWay);

            if (bridge.Equals("alipay", StringComparison.OrdinalIgnoreCase))
            {
                payway = PayWay.Alipay;
            }
            else if (bridge.Equals("tenpay", StringComparison.OrdinalIgnoreCase))
            {
                payway = PayWay.Tenpay;
            }

            return payway;
        }

        public static PayWay GetPaywayFromFlg(int flg)
        {
            switch (flg)
            {
                case 10:
                    return PayWay.Alipay;
                case 11:
                    return PayWay.Tenpay;
                case 12:
                    return PayWay.Ripple;
                default:
                    return default(PayWay);
            }
        }

        public static int ConvertPaywayFlg(PayWay payway)
        {
            switch (payway)
            {
                case PayWay.Alipay:
                    return 10;
                case PayWay.Tenpay:
                    return 11;
                case PayWay.Ripple:
                    return 12;
                default:
                    return 0;
            }
        }

        #region otp key
        /// <summary>
        /// generate one time password key
        /// </summary>                                                 
        /// <returns></returns>
        public static string GenerateOTPKey()
        {
            OathNet.Key key = new OathNet.Key();
            return key.Base32;
        }
        #endregion

        #region otp
        /// <summary>
        /// generate one time password for google authentication
        /// </summary>
        /// <param name="keyStr">keyStr</param>                            
        /// <returns></returns>
        public static string GenerateGoogleAuthOTP(string keyStr)
        {
            int otpDigits = 6;

            var key = new OathNet.Key(keyStr);
            var otp = new OathNet.TimeBasedOtpGenerator(key, otpDigits);
            return otp.GenerateOtp(DateTime.UtcNow);
        }

        /// <summary>
        /// generate one time password for sms
        /// </summary>
        /// <param name="counter">counter</param>
        /// <returns></returns>
        public static string GenerateSmsOTP(string keyStr, int counter)
        {
            int otpDigits = 6;

            var key = new OathNet.Key(keyStr);
            var otp = new OathNet.CounterBasedOtpGenerator(key, otpDigits);
            return otp.GenerateOtp(counter);
        }
        #endregion
    }
}
