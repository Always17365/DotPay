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
        /// generate exchange and queue name of generate virtual coin address 
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary>
        /// <param name="CurrencyType">currency</param>
        /// <returns></returns>
        public static Tuple<string, string> GenerateExchangeAndQueueNameOfGenerateNewAddress(CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            var exchangeName = currency.ToString() + "_GENERATE_ADDRESS_EXCHANGE";
            var queueName = currency.ToString() + "_GENERATE_ADDRESS_QUEUE";

            return new Tuple<string, string>(exchangeName, queueName);
        }

        /// <summary>
        /// generate exchange and queue name of create payment address
        /// <para>tuple's item1 is exchange name and item2 is queue name</para>
        /// </summary>
        /// <param name="CurrencyType">currency</param>
        /// <returns></returns>
        public static Tuple<string, string> GenerateExchangeAndQueueNameOfCreatePaymentAddress(CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            var exchangeName = currency.ToString() + "_CREATE_ADDRESS_EXCHANGE";
            var queueName = currency.ToString() + "_CREATE_ADDRESS_QUEUE";

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
