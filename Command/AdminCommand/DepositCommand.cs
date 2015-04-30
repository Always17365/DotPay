using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using DFramework.DynamicReflection;
using DFramework.Utilities;
using Dotpay.Common;

namespace Dotpay.AdminCommand
{
    #region Confirm Deposit Command
    public class ConfirmDepositCommand : Command<ErrorCode>
    {
        public ConfirmDepositCommand(Guid depositId, decimal amount, string transferNo, Guid confrimBy)
        {
            Check.Argument.IsNotEmpty(depositId, "userId");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");
            Check.Argument.IsNotEmpty(transferNo, "transferNo");
            Check.Argument.IsNotEmpty(confrimBy, "lockBy");

            this.DepositId = depositId;
            this.Amount = amount;
            this.TransferNo = transferNo;
            this.ConfrimBy = confrimBy;
        }

        public Guid DepositId { get; private set; }
        public decimal Amount { get; private set; }
        public string TransferNo { get; private set; }
        public Guid ConfrimBy { get; private set; }
    }
    //暂时不考虑确认失败，当未处理的交易过多时，可以考虑增加清理命令
    //public class ConfirmDepositFailCommand : Command<ErrorCode>
    //{
    //    public ConfirmDepositFailCommand(Guid depositId, string reasion, Guid confrimBy)
    //    {
    //        Check.Argument.IsNotEmpty(depositId, "userId");
    //        Check.Argument.IsNotEmpty(reasion, "reasion");
    //        Check.Argument.IsNotEmpty(confrimBy, "lockBy");

    //        this.DepositId = depositId; 
    //        this.Reason = reasion;
    //        this.ConfrimBy = confrimBy;
    //    }

    //    public Guid DepositId { get; private set; } 
    //    public string Reason { get; private set; }
    //    public Guid ConfrimBy { get; private set; }
    //}
    #endregion
}
