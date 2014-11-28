using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Create CNY Deposit Command
    public class CreateCommonCNYDeposit : FC.Framework.Command
    {
        public CreateCommonCNYDeposit(int userID, decimal amount, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNegative(currentUserID, "currentUserID");

            this.UserID = userID;
            this.Amount = amount;
            this.CreateBy = currentUserID;
        }

        public int UserID { get; protected set; }
        public decimal Amount { get; protected set; }
        public int CreateBy { get; protected set; }
    }

    public class CreateInboundCNYDeposit : FC.Framework.Command
    {
        public CreateInboundCNYDeposit(int userID, decimal amount, PayWay sourcePayway, string txid)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            this.UserID = userID;
            this.SourcePayway = sourcePayway;
            this.Amount = amount;
            this.InboundTxId = txid;
        }

        public int UserID { get; protected set; }
        public PayWay SourcePayway { get; protected set; }
        public decimal Amount { get; protected set; }
        public string InboundTxId { get; protected set; }
    }
    #endregion

    #region Complete CNY Deposit Command
    public class CompleteCNYDeposit : FC.Framework.Command
    {
        public CompleteCNYDeposit(int depositID, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(depositID, "depositID");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.DepositID = depositID;
            this.ByUserID = byUserID;
        }

        public int DepositID { get; protected set; }
        public int ByUserID { get; protected set; }
    }
    #endregion

    #region Undo  Complete CNY Deposit Command
    public class UndoCompleteCNYDeposit : FC.Framework.Command
    {
        public UndoCompleteCNYDeposit(int depositID, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(depositID, "depositID");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.DepositID = depositID;
            this.ByUserID = byUserID;
        }

        public int DepositID { get; protected set; }
        public int ByUserID { get; protected set; }
    }
    #endregion
}
