using FC.Framework;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{
    #region virtual coin deposit events

    #region deposit created
    public class VirtualCoinDepositCreated : DomainEvent
    {
        public VirtualCoinDepositCreated(int userID, int accountID, string txid, CurrencyType currency, decimal amount, decimal fee, string memo, VirtualCoinDeposit deposit)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.TxID = txid;
            this.Currency = currency;
            this.DepositAmount = amount;
            this.DepositFee = fee;
            this.Memo = memo;
            this.DepositEntity = deposit;
        }
        public int UserID { get; private set; }
        public int AccountID { get; private set; }
        public string TxID { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal DepositAmount { get; private set; }
        public decimal DepositFee { get; private set; }
        public string Memo { get; private set; }
        public VirtualCoinDeposit DepositEntity { get; private set; }
    }


    //public class VirtualCoinDepositSetFee : DomainEvent
    //{
    //    public VirtualCoinDepositSetFee(VirtualCoinDeposit deposit, CurrencyType currency, decimal fee)
    //    {
    //        this.VirtualCoinDeposit = deposit;
    //        this.Currency = currency;
    //        this.Fee = fee;
    //    }

    //    public decimal Fee { get; private set; }
    //    public VirtualCoinDeposit VirtualCoinDeposit { get; private set; }
    //    public CurrencyType Currency { get; private set; }
    //}
    #endregion

    #region deposit verified
    public class VirtualCoinDepositVerified : DomainEvent
    {
        public VirtualCoinDepositVerified(CurrencyType currency, int depositID, string depositUniqueID, int byUserID, string txid, decimal txAmount)
        {
            this.Currency = currency;
            this.DepositID = depositID;
            this.DepositUniqueID = depositUniqueID;
            this.ByUserID = byUserID;
            this.TxID = txid;
            this.TxAmount = txAmount;
        }
        public CurrencyType Currency { get; private set; }
        public int DepositID { get; private set; }
        public string DepositUniqueID { get; private set; }
        public int ByUserID { get; private set; }
        public string TxID { get; private set; }
        public decimal TxAmount { get; private set; }
    }
    #endregion

    #region deposit completed

    public class VirtualCoinDepositCompleted : DomainEvent
    {
        public VirtualCoinDepositCompleted(int depositID, int depositUserID, int accountID, decimal depositAmount, CurrencyType currency, int byUserID)
        {
            this.DepositID = depositID;
            this.DepositUserID = depositUserID;
            this.Currency = currency;
            this.AccountID = accountID;
            this.DepositAmount = depositAmount;
            this.ByUserID = byUserID;
        }
        public int DepositID { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal DepositAmount { get; private set; }
        public int AccountID { get; private set; }
        public int ByUserID { get; private set; }
        public int DepositUserID { get; private set; }
    }
    #endregion
    #endregion
}
