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
        LoginPasswordError = UserErrorBase + 1,
        LoginNameOrPasswordError = UserErrorBase + 2,
        PaymentPasswordError = UserErrorBase + 3,
        PaymentPasswordNotInitialized = UserErrorBase + 4,
        UserAccountIsLocked = UserErrorBase + 5,
        OldLoginPasswordError = UserErrorBase + 6,
        InvalidActiveToken = UserErrorBase + 7,
        OldPaymentPasswordError = UserErrorBase + 8,
        RealNameVerifationHasPassed = UserErrorBase + 9,
        UserMobileIsNotSet = UserErrorBase + 10,
        ExceedMaxPaymentPasswordFailTime = UserErrorBase + 11,
        SmsPasswordError = UserErrorBase + 12,
        MobileHasSet = UserErrorBase + 13,
        ExceedMaxLoginFailTime = UserErrorBase + 14,
        ExceedMaxResetLoginPasswordRequestTime = UserErrorBase + 15,      //登陆密码重置次数过多，控制Email发送数量
        ExceedMaxResetPaymentPasswordRequestTime = UserErrorBase + 16,    //支付密码重置次数过多，控制Email发送数量
        InvalidResetLoginPasswordToken = UserErrorBase + 17,              //错误登陆密码重置Token
        InvalidResetPaymentPasswordToken = UserErrorBase + 18,            //错误支付密码重置Token
        InvalidUser = UserErrorBase + 19,                                 //无效的用户
        UnactiveUser = UserErrorBase + 20,                                //未激活的用户
        UserHasActived = UserErrorBase + 21,                              //用户已激活 
        UserActiveEmailSendFrequently = UserErrorBase + 22,               //用户激活发送过于频繁
        #region Manager
        UserManagerErrorBase = 1500,
        HasNoPermission = UserManagerErrorBase + 1,
        SuperManagerHasInitialized = UserManagerErrorBase + 2,
        #endregion
        #endregion

        #region Account
        AccountErrorBase = 2000,
        AccountHasInitilized = AccountErrorBase + 1,
        AccountBalanceNotEnough = AccountErrorBase + 2,
        InvalidAccount = AccountErrorBase + 3,
        #endregion

        #region Deposit
        DepositErrorBase = 3000,
        DepositAmountNotMatch = DepositErrorBase + 1,                        //充值金额不匹配
        #endregion

        #region Tranasfer
        TranasferErrorBase = 4000,
        TranasferTransactionIsLockedByOther = TranasferErrorBase + 1,                         //转账已被其它人在处理                                                                                  
        TranasferTransactionSourceAccountNotExist = TranasferErrorBase + 2,                   //转账来源账户不存在
        TranasferTransactionTargetAccountNotExist = TranasferErrorBase + 3,                   //目标账户不存在
        TranasferTransactionAmountNotMatch = TranasferErrorBase + 4,                          //转账金额不相符
        AutomaticTranasferTransactionCanNotProccessByManager = TranasferErrorBase + 5,        //自动处理的转账不可以被管理员人工处理

        #endregion

        #region Ripple
        RippleErrorBase = 5000,
        RippleTransactionNotInit = RippleErrorBase + 1,
        RippleTransactionNotPending = RippleErrorBase + 2,
        RippleTransactionNotSubmit = RippleErrorBase + 3,
        RippleTransactionAmountNotMatch = RippleErrorBase + 4,
        RippleTransactionInvoiceIdNotMatch = RippleErrorBase + 5,
        RippleQuoteAmountOutOfRange = RippleErrorBase + 6,
        RippleQuoteUnsupport = RippleErrorBase + 7,
        RippleToFiErrorBase = RippleErrorBase + 200,
        RippleToFiIsLockedByOther = RippleToFiErrorBase + 1,
        #endregion

        #region Orleans Error
        OrleansErrorBase = 9000,
        DepositTransactionManagerError = OrleansErrorBase + 1
        #endregion

    }
}
