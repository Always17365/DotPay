using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Create Currency Command
    [ExecuteSync]
    public class CreateCurrency : FC.Framework.Command
    {
        public CreateCurrency(int currencyID, string code, string name, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currencyID, "currencyID");
            Check.Argument.IsNotEmpty(code, "code");
            Check.Argument.IsNotEmpty(name, "name");
            //Check.Argument.IsNotNegative(needConfirm, "needConfirm");
            //Check.Argument.IsNotNegative(depositFixedFee, "depositFixedFee");
            //Check.Argument.IsNotNegative(depositFeeRate, "depositFeeRate");
            //Check.Argument.IsNotNegative(withdrawFixedFee, "withdrawFixedFee");
            //Check.Argument.IsNotNegative(withdrawFeeRate, "withdrawFeeRate");
            //Check.Argument.IsNotNegative(withdrawDayLimit, "withdrawDayLimit");
            //Check.Argument.IsNotNegative(withdrawOnceLimit, "withdrawOnceLimit");
            //Check.Argument.IsNotNegative(withdrawVerifyLine, "withdrawVerifyLine");
            //Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.CurrencyID = currencyID;
            this.CurrencyCode = code;
            this.CurrencyName = name;
            //this.NeedConfirm = needConfirm;
            //this.DepositFeeRate = depositFeeRate;
            //this.DepositFixedFee = depositFixedFee;
            //this.WithdrawFeeRate = withdrawFeeRate;
            //this.WithdrawFixedFee = withdrawFixedFee;
            //this.WithdrawDayLimit = withdrawDayLimit;
            //this.WithdrawOnceLimit = withdrawOnceLimit;
            //this.WithdrawVerifyLine = withdrawVerifyLine;
            this.CreateBy = currentUserID;
        }
        public int CurrencyID { get; private set; }
        public string CurrencyCode { get; private set; }
        public string CurrencyName { get; private set; }
        //public int NeedConfirm { get; private set; }
        //public decimal DepositFeeRate { get; private set; }
        //public decimal DepositFixedFee { get; private set; }
        //public decimal WithdrawFeeRate { get; private set; }
        //public decimal WithdrawFixedFee { get; private set; }
        //public int WithdrawDayLimit { get; private set; }
        //public int WithdrawOnceLimit { get; private set; }
        //public int WithdrawVerifyLine { get; private set; }
        public int CreateBy { get; private set; }
    }
    #endregion

    #region Enable Currency Command
    [ExecuteSync]
    public class EnableCurrency : FC.Framework.Command
    {
        public EnableCurrency(int currencyID, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currencyID, "currencyID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.CurrencyID = currencyID;
            this.ByUserID = currentUserID;
        }
        public int CurrencyID { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Enable Currency Command
    [ExecuteSync]
    public class DisableCurrency : FC.Framework.Command
    {
        public DisableCurrency(int currencyID, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currencyID, "currencyID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.CurrencyID = currencyID;
            this.ByUserID = currentUserID;
        }
        public int CurrencyID { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Modify DepositFeeRate Command
    [ExecuteSync]
    public class ModifyCurrencyDepositFeeRate : FC.Framework.Command
    {
        public ModifyCurrencyDepositFeeRate(int currencyID, decimal depositFixedFee, decimal depositFeeRate, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currencyID, "currencyID");
            Check.Argument.IsNotNegative(depositFixedFee, "depositFixedFee");
            Check.Argument.IsNotNegative(depositFeeRate, "depositFeeRate");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.CurrencyID = currencyID;
            this.DepositFixedFee = depositFixedFee;
            this.DepositFeeRate = depositFeeRate;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal DepositFixedFee { get; private set; }
        public decimal DepositFeeRate { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Modify WithdrawFeeRate Command
    [ExecuteSync]
    public class ModifyCurrencyWithdrawFeeRate : FC.Framework.Command
    {
        public ModifyCurrencyWithdrawFeeRate(int currencyID, decimal wthdrawFixedFee, decimal wthdrawFeeRate, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currencyID, "currencyID");
            Check.Argument.IsNotNegative(wthdrawFixedFee, "wthdrawFixedFee");
            Check.Argument.IsNotNegative(wthdrawFeeRate, "wthdrawFeeRate");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.CurrencyID = currencyID;
            this.WithdrawFixedFee = wthdrawFixedFee;
            this.WithdrawFeeRate = wthdrawFeeRate;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal WithdrawFixedFee { get; private set; }
        public decimal WithdrawFeeRate { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Modify WithdrawFeeRate Command
    [ExecuteSync]
    public class ModifyCurrencyNeedConfirm : FC.Framework.Command
    {
        public ModifyCurrencyNeedConfirm(int currencyID, int needConfirm, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currencyID, "currencyID");
            Check.Argument.IsNotNegative(needConfirm, "needConfirm");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.CurrencyID = currencyID;
            this.NeedConfirm = needConfirm;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public int NeedConfirm { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Modify Currency Withdraw VerifyLine Command
    [ExecuteSync]
    public class ModifyCurrencyWithdrawVerifyLine : FC.Framework.Command
    {
        public ModifyCurrencyWithdrawVerifyLine(int currencyID, decimal verifyLine, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currencyID, "currencyID");
            Check.Argument.IsNotNegative(verifyLine, "verifyLine");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.CurrencyID = currencyID;
            this.VerifyLine = verifyLine;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal VerifyLine { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Modify Currency Withdraw DayLimit Command
    [ExecuteSync]
    public class ModifyCurrencyWithdrawDayLimit : FC.Framework.Command
    {
        public ModifyCurrencyWithdrawDayLimit(int currencyID, decimal dayLimit, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currencyID, "currencyID");
            Check.Argument.IsNotNegative(dayLimit, "dayLimit");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.CurrencyID = currencyID;
            this.DayLimit = dayLimit;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal DayLimit { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Modify Currency Withdraw Once Limit Command
    [ExecuteSync]
    public class ModifyCurrencyWithdrawOnceLimit : FC.Framework.Command
    {
        public ModifyCurrencyWithdrawOnceLimit(int currencyID, decimal onceLimit, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currencyID, "currencyID");
            Check.Argument.IsNotNegative(onceLimit, "onceLimit");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.CurrencyID = currencyID;
            this.OnceLimit = onceLimit;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal OnceLimit { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Modify Currency Withdraw Once Min Command
    [ExecuteSync]
    public class ModifyCurrencyWithdrawOnceMin : FC.Framework.Command
    {
        public ModifyCurrencyWithdrawOnceMin(int currencyID, decimal onceMin, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currencyID, "currencyID");
            Check.Argument.IsNotNegative(onceMin, "onceLimit");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.CurrencyID = currencyID;
            this.OnceMin = onceMin;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal OnceMin { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion
}
