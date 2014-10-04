using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    public enum ScoreSource
    {
        Login = 1,
        Trade = 2,
        CNYDeposit = 3,
        BTCDeposit = 4,
        LTCDeposit = 5,
        NXTDeposit = 6,
        DOGEDeposit = 7,
        IFCDeposit = 8,
        STRDeposit = 9,
        FBCDeposit = 9,

        CNYTrade = 50,
        BTCTrade = 51,
        LTCTrade = 52,
        NXTTrade = 53,
        DOGETrade = 54,
        IFCTrade = 55,
        STRTrade = 56,
        FBCTrade = 57,

        Lottery = 101,

        CNYWithdraw = 201,
        BTCWithdraw = 202,
        LTCWithdraw = 203,
        NXTWithdraw = 204,
        DOGEWithdraw = 205,
        IFCWithdraw = 206,
        STRWithdraw = 207,
        FBCWithdraw = 208
    }
}
