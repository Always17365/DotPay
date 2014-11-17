using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    public enum ErrorCode
    {
        #region 用户相关
        UsernameAlreadyExists = 101,
        LoginNameOrPasswordError = 102,
        TradePasswordError = 103,
        UserAccountIsLocked = 104,
        OldPasswordError = 105,
        TokenIsUsedOrTimeOut = 106,
        GAPasswordError = 107,
        OldTradePasswordError = 108,
        RealNameAuthenticationIsPassed = 109,
        UserIDOfTokenNotMatch = 111,
        TokenNotMatch = 112,
        GoogleAuthenticationIsSetted = 113,
        GoogleAuthenticationIsNotSet = 114,
        UserMobileIsNotSet = 115,
        SMSPasswordError = 116,
        MobileHasSet = 99,
        UserHasNo2FA = 98,
        EmailHasVerified = 97,
        #endregion

        CurrencyIsLocked = 117,
        CurrencyIsDisabled = 118,

        AccountBalanceNotEnough = 121,
        AccountLockedNotEnough = 122,

        FundSourceIsVerified = 130,
        FundSourceIsNotVerified = 131,
        FundSourceIsProcessing = 132,
        FundSourceIsNotProcessing = 133,
        FundSourceIsCompleted = 134,
        FundSourceIsDeleted = 135,
        PaymentTransactionIsCompleted = 136,
        FundSourceIsNotCompleted = 137,
        PaymentTransactionAmountError = 138,


        DepositNotVerify = 140,
        DepositIsVerified = 141,
        DepositIsCompleted = 142,
        DepositIsNotComplete = 143,


        DepositNotEqualsFundAmount = 145,
        AccountAmountNotEnoughBecauseUsed = 146,


        WithdrawNotVerify = 160,
        WithdrawIsVerified = 161,
        WithdrawIsProcessing = 162,
        WithdrawIsNotSubmitToProcess = 163,
        WithdrawIsCompleted = 164,
        WithdrawIsNotComplete = 165,
        WithdrawIsFailed = 166,
        WithdrawIsOverDayLimit = 167,
        WithdrawIsCanceled = 168,
        WithdrawAmountOutOfRange = 169,

        RippleTransactionNotPending = 201,



        TransferTransactionNotPending=301,

        NotAllowAssignSuperManager = 901,
        NoPermission = 902,
    }
}
