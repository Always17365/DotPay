using System;
using System.Collections.Generic;
using DFramework.Utilities;
using Dotpay.Common;
using Dotpay.Common.Enum;

namespace Dotpay.Command
{
    #region Create Deposit Command
    public class CreateDepositCommand : Command<Tuple<ErrorCode, string>>
    {
        public CreateDepositCommand(Guid accountId, CurrencyType currency, decimal amount, Payway payway)
        {
            Check.Argument.IsNotEmpty(accountId, "accountId"); 
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");
            Check.Argument.IsNotNegativeOrZero((int)payway, "payway");

            this.AccountId = accountId;
            this.Currency = currency;
            this.Amount = amount;
            this.Payway = payway; 
        }

        public Guid AccountId { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public Payway Payway { get; private set; }
    }
    #endregion

    #region Confirm Deposit Command
    public class ConfirmDepositCommand : Command<ErrorCode>
    {
        public ConfirmDepositCommand(Guid depositTxId, string transferNo, decimal amount)
        {
            Check.Argument.IsNotEmpty(depositTxId, "depositTxId");
            Check.Argument.IsNotEmpty(transferNo, "transferNo");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");

            this.DepositTxId = DepositTxId;
            this.TransferNo = transferNo;
            this.Amount = amount;
        }

        public Guid DepositTxId { get; private set; }
        public string TransferNo { get; private set; }
        public decimal Amount { get; private set; } 
    }
    #endregion
}
