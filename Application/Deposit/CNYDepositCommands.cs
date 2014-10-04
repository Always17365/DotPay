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
    public class CreateCNYDeposit : FC.Framework.Command
    {
        public CreateCNYDeposit(int userID, int fundSourceID, decimal amount,  int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.UserID = userID;
            this.FundSourceID = fundSourceID;
            this.Amount = amount;
            this.CreateBy = currentUserID;
        }

        public int UserID { get; protected set; }
        public decimal Amount { get; protected set; }
        public int FundSourceID { get; protected set; }
        public string Memo { get; protected set; }
        public int CreateBy { get; protected set; }
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
