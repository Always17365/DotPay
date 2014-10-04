using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Domain.Events;
using DotPay.Domain.Exceptions;
using DotPay.Common;

namespace DotPay.Domain
{
    public class NXTAccountVersion : AccountVersion
    {
        #region ctor
        protected NXTAccountVersion() { }

        public NXTAccountVersion(int userID, int accountID, decimal amount,
                              decimal balance, decimal locked, decimal @in, decimal @out,
                              int modifyID, int modifyType)
            : base(userID, accountID, amount, balance, locked, @in, @out, modifyID, modifyType) { }


        public NXTAccountVersion(int userID, int accountID, decimal amount,
                             decimal balance, decimal locked, decimal @in, decimal @out,
                             string depositCode, int modifyType)
            : base(userID, accountID, amount, balance, locked, @in, @out, depositCode, modifyType) { }
        #endregion
    }
}
