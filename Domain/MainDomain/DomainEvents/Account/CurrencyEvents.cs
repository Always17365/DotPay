using FC.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{
    public class CurrencyCreated : DomainEvent
    {
        public CurrencyCreated(int currencyID, string code, string name,  int createBy)
        {
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
            this.CreateBy = createBy;
        }
        public int CurrencyID { get; private set; }
        public string CurrencyCode { get; private set; }
        public string CurrencyName { get; private set; }
        public int NeedConfirm { get; private set; }
        public decimal DepositFeeRate { get; private set; }
        public decimal DepositFixedFee { get; private set; }
        public decimal WithdrawFeeRate { get; private set; }
        public decimal WithdrawFixedFee { get; private set; }
        public int WithdrawDayLimit { get; private set; }
        public int WithdrawOnceLimit { get; private set; }
        public int WithdrawVerifyLine { get; private set; }
        public int CreateBy { get; private set; }
    }

    public class CurrencyEnabled : DomainEvent
    {
        public CurrencyEnabled(int currencyID, string currencyCode, int byUserID)
        {
            this.CurrencyCode = currencyCode;
            this.CurrencyID = currencyID;
            this.ByUserID = byUserID;
        }
        public string CurrencyCode { get; private set; }
        public int CurrencyID { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class CurrencyDisabled : DomainEvent
    {
        public CurrencyDisabled(int currencyID, string currencyCode, int byUserID)
        {
            this.CurrencyCode = currencyCode;
            this.CurrencyID = currencyID;
            this.ByUserID = byUserID;
        }
        public string CurrencyCode { get; private set; }
        public int CurrencyID { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class CurrencyWithdrawRateModified : DomainEvent
    {
        public CurrencyWithdrawRateModified(int currencyID, decimal withdrawFixedFee, decimal withdrawFeeRate, int byUserID)
        {
            this.CurrencyID = currencyID;
            this.WithdrawFixedFee = withdrawFixedFee;
            this.WithdrawFeeRate = withdrawFeeRate;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal WithdrawFixedFee { get; private set; }
        public decimal WithdrawFeeRate { get; private set; }
        public int ByUserID { get; private set; }
    }
    public class CurrencyWithdrawVerifyLineModified : DomainEvent
    {
        public CurrencyWithdrawVerifyLineModified(int currencyID, decimal verifyLine, int byUserID)
        {
            this.CurrencyID = currencyID;
            this.VerifyLine = verifyLine;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal VerifyLine { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class CurrencyWithdrawDayLimitModified : DomainEvent
    {
        public CurrencyWithdrawDayLimitModified(int currencyID, decimal dayLimit, int byUserID)
        {
            this.CurrencyID = currencyID;
            this.DayLimit = dayLimit;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal DayLimit { get; private set; }
        public int ByUserID { get; private set; }

    }

    public class CurrencyWithdrawOnceMinModified : DomainEvent
    {
        public CurrencyWithdrawOnceMinModified(int currencyID, decimal onceMin, int byUserID)
        {
            this.CurrencyID = currencyID;
            this.OnceMin = onceMin;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal OnceMin { get; private set; }
        public int ByUserID { get; private set; }

    }
    public class CurrencyWithdrawOnceLimitModified : DomainEvent
    {
        public CurrencyWithdrawOnceLimitModified(int currencyID, decimal onceLimit, int byUserID)
        {
            this.CurrencyID = currencyID;
            this.OnceLimit = onceLimit;
            this.ByUserID = byUserID;
        }
        public int CurrencyID { get; private set; }
        public decimal OnceLimit { get; private set; }
        public int ByUserID { get; private set; }

    }
}
