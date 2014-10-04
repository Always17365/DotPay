using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    /// <summary>
    /// two factor type
    /// </summary>
    public enum TwoFactorType
    {
        none = 0,
        GoogleAuth = 1,
        Sms = 2
    }
}
