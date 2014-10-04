using FC.Framework;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Events
{
    #region cny deposit events
    #region deposit created
    public class CNYDepositCreated : DomainEvent
    {
        public CNYDepositCreated(int userID, int accountID, decimal sumAmount, string memo, DepositType depositType, CNYDeposit cnyDeposit)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.DepositSumAmount = sumAmount;
            this.Memo = memo;
            this.DepositType = depositType;
            this.CNYDepositEntity = cnyDeposit;
        }
        public int UserID { get; private set; }
        public int AccountID { get; private set; }
        public decimal DepositSumAmount { get; private set; }
        public string Memo { get; private set; }
        public DepositType DepositType { get; private set; }
        public CNYDeposit CNYDepositEntity { get; private set; }

    }

    public class CNYDepositSetFee : DomainEvent
    {
        public CNYDepositSetFee(CNYDeposit deposit, decimal amount, decimal fee)
        {
            this.CNYDeposit = deposit;
            this.Amount = amount;
            this.Fee = fee;
        }

        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
        public CNYDeposit CNYDeposit { get; private set; }
    }
    #endregion

    #region deposit verified
    public class CNYDepositVerified : DomainEvent
    {
        public CNYDepositVerified(int depositID, PayWay payway, Bank bank, string depositUniqueID, int byUserID, int fundSourceID, decimal fundAmount, string fundExtra)
        {
            this.DepositID = depositID;
            this.DepositUniqueID = depositUniqueID;
            this.ByUserID = byUserID;
            this.FundSourceID = fundSourceID;
            this.PayWay = payway;
            this.Bank = bank;
            this.FundAmount = fundAmount;
            this.FundExtra = fundExtra;
        }
        public int DepositID { get; private set; }
        public string DepositUniqueID { get; private set; }
        public int ByUserID { get; private set; }
        public PayWay PayWay { get; private set; }
        public Bank Bank { get; private set; }
        public int FundSourceID { get; private set; }
        public decimal FundAmount { get; private set; }
        public string FundExtra { get; private set; }

    }
    #endregion

    #region deposit completed

    public class CNYDepositCompleted : DomainEvent
    {
        public CNYDepositCompleted(int depositID, int depositUserID, int accountID, decimal depositAmount, int byUserID)
        {
            this.DepositID = depositID;
            this.DepositUserID = depositUserID;
            this.AccountID = accountID;
            this.DepositAmount = depositAmount;
            this.ByUserID = byUserID;
        }
        public int DepositID { get; private set; }
        public decimal DepositAmount { get; private set; }
        public int AccountID { get; private set; }
        public int ByUserID { get; private set; }
        public int DepositUserID { get; private set; }
        public CurrencyType CurrencyType { get { return CurrencyType.CNY; } }
    }
    #endregion

    #region deposit  undo complete
    public class CNYDepositUndoComplete : DomainEvent
    {
        public CNYDepositUndoComplete(int depositID,int depositUserID, int accountID, int fundSourceID, decimal depositAmount, int byUserID)
        {
            this.DepositID = depositID;
            this.DepositUserID = depositUserID;
            this.AccountID = accountID;
            this.FundSourceID = fundSourceID;
            this.DepositAmount = depositAmount;
            this.ByUserID = byUserID;
        }
        public int DepositID { get; private set; }
        public int DepositUserID { get; private set; }
        public int FundSourceID { get; private set; }
        public decimal DepositAmount { get; private set; }
        public int AccountID { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region deposit deleted
    public class CNYDepositDeleted : DomainEvent
    {
        public CNYDepositDeleted(int depositID, int byUserID, int fundSourceID)
        {
            this.DepositID = depositID;
            this.ByUserID = byUserID;
            this.FundSourceID = fundSourceID;
        }
        public int DepositID { get; private set; }
        public int ByUserID { get; private set; }
        public int FundSourceID { get; private set; }
    }
    #endregion
    #endregion
}
