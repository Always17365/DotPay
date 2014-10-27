using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Create Account Command
    [ExecuteAsync]
    public class CreateAccount : FC.Framework.Command
    {
        public CreateAccount(CurrencyType currency, int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNull(currency, "currency");

            this.Currency = currency;
            this.UserID = userID;
        }
        public CurrencyType Currency { get; private set; }
        public int UserID { get; private set; }
    }
    #endregion

}
