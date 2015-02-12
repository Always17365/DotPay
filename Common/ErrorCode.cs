using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotpay.Common
{
    public enum ErrorCode
    {
        None = 0,
        Unknow = 1,
        #region user 
        EmailAlreadyRegisted = 100,
        UserAccountAlreadyExists,
        LoginNameOrPasswordError,
        PaymentPasswordError,
        UserAccountIsLocked,
        OldLoginPasswordError,
        TokenIsUsedOrTimeOut,
        OldPaymentPasswordError,
        RealNameVerifationHasPassed,
        UserMobileIsNotSet,
        SMSPasswordError,
        EmailHasVerified,
        MobileHasSet,
        #endregion

        #region ripple
        RippleTransactionNotInit = 201,
        RippleTransactionNotPending ,
        RippleTransactionNotSubmit ,
        RippleTransactionAmountNotMatch  ,
        RippleTransactionInvoiceIdNotMatch,
        RippleQuoteAmountOutOfRange  
        #endregion


    }
}
