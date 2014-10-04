using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    // 不需要实现执行器
    [ExecuteDistributed]
    public class GeneratePaymentAddress : FC.Framework.Command
    {
        public GeneratePaymentAddress(int userID, CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.UserID = userID;
            this.Currency = currency;
        }

        public int UserID { get; private set; }
        public CurrencyType Currency { get; private set; }
    }

    [ExecuteSync]
    public class CreatePaymentAddress : FC.Framework.Command
    {
        public CreatePaymentAddress(int userID, string paymentAddress, CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(paymentAddress, "paymentAddress");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.UserID = userID;
            this.PaymentAddress = paymentAddress;
            this.Currency = currency;
        }

        public int UserID { get; private set; }
        public string PaymentAddress { get; private set; }
        public CurrencyType Currency { get; private set; }
    }
}
