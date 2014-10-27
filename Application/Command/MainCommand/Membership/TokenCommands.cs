using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Token Use
    [ExecuteAsync]
    public class TokenUse: FC.Framework.Command
    {
        public TokenUse(int tokenID)
        {
            Check.Argument.IsNotNegativeOrZero(tokenID, "tokenID");

            this.TokenID = tokenID;
        }

        public int TokenID { get; private set; }
    }
    #endregion
}
