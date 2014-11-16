using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    public class InsideTransfer : FC.Framework.Command
    {
        public InsideTransfer(int fromUserID, int toUserID, CurrencyType currency, decimal amount, string description)
        {
            Check.Argument.IsNotNegativeOrZero(fromUserID, "fromUserID");
            Check.Argument.IsNotNegativeOrZero(toUserID, "toUserID");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.FromUserID = fromUserID;
            this.ToUserID = toUserID;
            this.Currency = currency;
            this.Amount = amount;
            this.Description = description;
        }

        public int FromUserID { get; private set; }
        public int ToUserID { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
    }

    public class InsideTransferComplete : FC.Framework.Command
    {
        public InsideTransferComplete(int insideTransferID, CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero(insideTransferID, "insideTransferID");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.InsideTransferID = insideTransferID;
            this.Currency = currency;
        }

        public int InsideTransferID { get; private set; }
        public CurrencyType Currency { get; private set; }
    }


    public class OutsideTransfer : FC.Framework.Command
    {
        public OutsideTransfer(int userID, CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.UserID = userID;
            this.Currency = currency;
        }

        public int UserID { get; private set; }
        public CurrencyType Currency { get; private set; }
    }

    public class ExchangeTransfer : FC.Framework.Command
    {
        public ExchangeTransfer(int userID, CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.UserID = userID;
            this.Currency = currency;
        }

        public int UserID { get; private set; }
        public CurrencyType Currency { get; private set; }
    }
}
