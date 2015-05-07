using System;
using DFramework;
using DFramework.Utilities;
using Dotpay.Common;

namespace Dotpay.AdminCommand
{
    #region Lock Transfer Transaction Command
    public class LockTransferTransactionCommand : Command<ErrorCode>
    {
        public LockTransferTransactionCommand(Guid transferTransactionId, Guid lockBy )
        {
            Check.Argument.IsNotEmpty(transferTransactionId, "transferTransactionId");
            Check.Argument.IsNotEmpty(lockBy, "lockBy"); 

            this.TransferTransactionId = transferTransactionId;
            this.LockBy = lockBy; 
        }

        public Guid TransferTransactionId { get; private set; }
        public Guid LockBy { get; private set; } 
    } 
    public class ConfirmTransferTransactionSuccessCommand : Command<ErrorCode>
    {
        public ConfirmTransferTransactionSuccessCommand(Guid transferTransactionId, decimal amount,string transferNo, Guid confrimBy)
        {
            Check.Argument.IsNotEmpty(transferTransactionId, "transferTransactionId");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");
            Check.Argument.IsNotEmpty(transferNo, "transferNo");
            Check.Argument.IsNotEmpty(confrimBy, "confrimBy");

            this.TransferTransactionId = transferTransactionId;
            this.Amount = amount;
            this.FiTransferNo = transferNo;
            this.ConfrimBy = confrimBy;
        }

        public Guid TransferTransactionId { get; private set; }
        public decimal Amount { get; private set; }
        public string FiTransferNo { get; private set; }
        public Guid ConfrimBy { get; private set; }
    }

    public class ConfirmTransferTransactionFailCommand : Command<ErrorCode>
    {
        public ConfirmTransferTransactionFailCommand(Guid transferTransactionId, string reasion, Guid confrimBy)
        {
            Check.Argument.IsNotEmpty(transferTransactionId, "userId");
            Check.Argument.IsNotEmpty(reasion, "reasion");
            Check.Argument.IsNotEmpty(confrimBy, "lockBy");

            this.TransferTransactionId = transferTransactionId;
            this.Reason = reasion;
            this.ConfrimBy = confrimBy;
        }

        public Guid TransferTransactionId { get; private set; }
        public string Reason { get; private set; }
        public Guid ConfrimBy { get; private set; }
    }
    #endregion
}
