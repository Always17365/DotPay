using System;
using System.Collections.Generic;
using DFramework.Utilities;
using Dotpay.Common;
using Dotpay.Common.Enum;

namespace Dotpay.Command
{
    #region Submit Transfer Transaction Command
    public class SubmitTransferToDotpayTransactionCommand : DFramework.Command<ErrorCode>
    {
        public SubmitTransferToDotpayTransactionCommand(Guid transferTransactionId, Guid sourceAccountId, Guid targetAccountId, CurrencyType currency, decimal amount, string memo, string realName,string paymentPassword)
        {
            Check.Argument.IsNotEmpty(transferTransactionId, "transferTransactionId");
            Check.Argument.IsNotEmpty(sourceAccountId, "sourceAccountId");
            Check.Argument.IsNotEmpty(targetAccountId, "targetAccountId");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");
            Check.Argument.IsNotEmpty(paymentPassword, "paymentPassword");

            this.SourceAccountId = sourceAccountId;
            this.TargetAccountId = targetAccountId;
            this.Currency = currency;
            this.Amount = amount;
            this.Memo = memo;
            this.RealName = realName;
            this.PaymentPassword = paymentPassword;
            this.TransferTransactionId = transferTransactionId;
        }

        public Guid TransferTransactionId { get; private set; }
        public Guid SourceAccountId { get; private set; }
        public Guid TargetAccountId { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public string Memo { get; private set; }
        public string RealName { get; private set; }
        public string PaymentPassword { get; private set; }
    }
    public class SubmitTransferToAlipayTransactionCommand : DFramework.Command<ErrorCode>
    {
        public SubmitTransferToAlipayTransactionCommand(Guid transferTransactionId,Guid sourceAccountId, string targetAccount, Payway payway, CurrencyType currency, decimal amount, string memo, string realName, string paymentPassword)
        {
            Check.Argument.IsNotEmpty(transferTransactionId, "transferTransactionId");
            Check.Argument.IsNotEmpty(sourceAccountId, "sourceAccountId");
            Check.Argument.IsNotEmpty(targetAccount, "targetAccount");
            Check.Argument.IsNotNegativeOrZero((int)payway, "payway");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");

            this.TransferTransactionId = transferTransactionId;
            this.SourceAccountId = sourceAccountId;
            this.TargetAccount = targetAccount;
            this.Currency = currency;
            this.Payway = payway;
            this.Amount = amount;
            this.Memo = memo;
            this.RealName = realName;
            this.PaymentPassword = paymentPassword;
        }

        public Guid TransferTransactionId { get; private set; }
        public Guid SourceAccountId { get; private set; }
        public string TargetAccount { get; private set; }
        public string RealName { get; private set; }
        public Payway Payway { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public string Memo { get; private set; }
        public string PaymentPassword { get; private set; }
    }

    public class SubmitTransferToBankTransactionCommand : DFramework.Command<ErrorCode>
    {
        public SubmitTransferToBankTransactionCommand(Guid transferTransactionId, Guid sourceAccountId, string targetAccount, string targetRealName, Bank bank, CurrencyType currency, decimal amount, string memo, string paymentPassword)
        {
            Check.Argument.IsNotEmpty(transferTransactionId, "transferTransactionId");
            Check.Argument.IsNotEmpty(sourceAccountId, "sourceAccountId");
            Check.Argument.IsNotEmpty(targetAccount, "targetAccount");
            Check.Argument.IsNotEmpty(targetRealName, "targetRealName");
            Check.Argument.IsNotNegativeOrZero((int)bank, "bank");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");

            this.TransferTransactionId = transferTransactionId;
            this.SourceAccountId = sourceAccountId;
            this.TargetRealName = targetRealName;
            this.TargetAccount = targetAccount;
            this.Bank = bank;
            this.Currency = currency;
            this.Amount = amount;
            this.Memo = memo;
            this.PaymentPassword = paymentPassword;
        }

        public Guid TransferTransactionId { get; private set; }
        public Guid SourceAccountId { get; private set; }
        public string TargetRealName { get; private set; }
        public string TargetAccount { get; private set; }
        public Bank Bank { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public string Memo { get; private set; }
        public string PaymentPassword { get; private set; }
    }

    public class SubmitTransferToRippleTransactionCommand : DFramework.Command<ErrorCode>
    {
        public SubmitTransferToRippleTransactionCommand(Guid transferTransactionId, Guid sourceAccountId, string rippleAddress, CurrencyType currency, decimal amount, string memo, string paymentPassword)
        {
            Check.Argument.IsNotEmpty(transferTransactionId, "transferTransactionId");
            Check.Argument.IsNotEmpty(sourceAccountId, "sourceAccountId");
            Check.Argument.IsNotEmpty(rippleAddress, "rippleAddress");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");

            this.TransferTransactionId = transferTransactionId;
            this.SourceAccountId = sourceAccountId;
            this.RippleAddress = rippleAddress;
            this.Currency = currency;
            this.Amount = amount;
            this.Memo = memo;
            this.PaymentPassword = paymentPassword;
        }
        public Guid TransferTransactionId { get; private set; }
        public Guid SourceAccountId { get; private set; }
        public string RippleAddress { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public string Memo { get; private set; }
        public string PaymentPassword { get; private set; }
    }
    #endregion 
}
