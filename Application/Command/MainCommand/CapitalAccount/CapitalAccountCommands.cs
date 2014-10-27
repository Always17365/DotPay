using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Create Capital Account Command
    public class CreateCapitalAccount : FC.Framework.Command
    {
        public CreateCapitalAccount(string bankAccount, string ownerName, Bank bank, int createBy)
        {
            Check.Argument.IsNotEmpty(bankAccount, "bankAccount");
            Check.Argument.IsNotEmpty(ownerName, "ownerName");
            Check.Argument.IsNotNegativeOrZero(createBy, "createBy");

            this.BankAccount = bankAccount;
            this.OwnerName = ownerName;
            this.Bank = bank;
            this.CreateBy = createBy;
        }

        public Bank Bank { get; private set; }
        public string OwnerName { get; private set; }
        public string BankAccount { get; private set; }
        public int CreateBy { get; private set; }
    }
    #endregion

    #region Enable Capital Account Command
    public class EnableCapitalAccount : FC.Framework.Command
    {
        public EnableCapitalAccount(int capitalAccountID, int enableBy)
        {
            Check.Argument.IsNotNegativeOrZero(capitalAccountID, "capitalAccountID");
            Check.Argument.IsNotNegativeOrZero(enableBy, "enableBy");

            this.CapitalAccountID = capitalAccountID;
            this.EnableBy = enableBy;
        }

        public int CapitalAccountID { get; private set; }
        public int EnableBy { get; private set; }
    }
    #endregion

    #region Disable Capital Account Command
    public class DisableCapitalAccount : FC.Framework.Command
    {
        public DisableCapitalAccount(int capitalAccountID, int disableBy)
        {
            Check.Argument.IsNotNegativeOrZero(capitalAccountID, "capitalAccountID");
            Check.Argument.IsNotNegativeOrZero(disableBy, "disableBy");

            this.CapitalAccountID = capitalAccountID;
            this.DisableBy = disableBy;
        }

        public int CapitalAccountID { get; private set; }
        public int DisableBy { get; private set; }
    }
    #endregion
}
