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
        UserErrorBase = 1000,
        EmailAlreadyRegisted = UserErrorBase + 1,
        UserAccountAlreadyExists = UserErrorBase + 2,
        LoginNameOrPasswordError = UserErrorBase + 3,
        PaymentPasswordError = UserErrorBase + 4,
        UserAccountIsLocked = UserErrorBase + 5,
        OldLoginPasswordError = UserErrorBase + 6,
        TokenIsUsedOrTimeOut = UserErrorBase + 7,
        OldPaymentPasswordError = UserErrorBase + 8,
        RealNameVerifationHasPassed = UserErrorBase + 9,
        UserMobileIsNotSet = UserErrorBase + 10,
        SmsPasswordError = UserErrorBase + 11,
        EmailHasVerified = UserErrorBase + 12,
        MobileHasSet = UserErrorBase + 13,
        #endregion

        #region Account
        AccountErrorBase = 2000,
        AccountHasInitilized = AccountErrorBase + 1,
        AccountBalanceNotEnough = AccountErrorBase + 2,
        #endregion

        #region Deposit
        DepositErrorBase = 3000,
        DepositIsLockedByOther = DepositErrorBase + 1, 
        #endregion

        #region TranasferToFI
        TranasferToFiErrorBase = 3000,
        TranasferToFiIsLockedByOther = TranasferToFiErrorBase + 1, 
        #endregion

        #region ripple
        RippleTransactionNotInit = 201,
        RippleTransactionNotPending,
        RippleTransactionNotSubmit,
        RippleTransactionAmountNotMatch,
        RippleTransactionInvoiceIdNotMatch,
        RippleQuoteAmountOutOfRange,
        #endregion

        #region Orleans Error
        OrleansErrorBase = 9000,
        DepositTransactionManagerError = OrleansErrorBase + 1
        #endregion

    }
}
