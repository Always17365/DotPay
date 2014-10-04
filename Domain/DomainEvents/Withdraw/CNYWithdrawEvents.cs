using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Events
{
    public class CNYWithdrawCreated : DomainEvent
    {
        public CNYWithdrawCreated(int withdrawUserID, int accountID, decimal amount, int userBankAccountID, CNYWithdraw withdrawEntity)
        {
            this.WithdrawUserID = withdrawUserID;
            this.AccountID = accountID;
            this.Amount = amount;
            this.UserBankAccountID = userBankAccountID;
            this.CNYWithdrawEntity = withdrawEntity;
        }

        public int WithdrawUserID { get; private set; }
        public int AccountID { get; private set; }
        public decimal Amount { get; private set; }
        public int UserBankAccountID { get; private set; }
        public CNYWithdraw CNYWithdrawEntity { get; private set; }
    }

    public class CNYWithdrawSetFee : DomainEvent
    {
        public CNYWithdrawSetFee(CNYWithdraw withdraw, decimal fee)
        {
            this.CNYWithdraw = withdraw;
            this.Fee = fee;
        }

        public decimal Fee { get; private set; }
        public CNYWithdraw CNYWithdraw { get; private set; }
    }

    public class CNYWithdrawVerified : DomainEvent
    {
        public CNYWithdrawVerified(int cnyWithdrawID, decimal amount, decimal fee, string memo, int byUserID)
        {
            this.WithdrawID = cnyWithdrawID;
            this.Amount = amount;
            this.Memo = memo;
            this.Fee = fee;
            this.ByUserID = byUserID;
        }

        public int WithdrawID { get; private set; }
        public int ByUserID { get; private set; }
        public string Memo { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
    }

    public class CNYWithdrawSkipVerify : DomainEvent
    {
        public CNYWithdrawSkipVerify(int cnyWithdrawID)
        {
            this.WithdrawID = cnyWithdrawID;
        }

        public int WithdrawID { get; private set; }
    }

    public class CNYWithdrawModifiedReceiverBankAccount : DomainEvent
    {
        public CNYWithdrawModifiedReceiverBankAccount(int withdrawID, int bankAccountID, int byUserID)
        {
            this.WithdrawID = withdrawID;
            this.ReceiverBankAccountID = bankAccountID;
            this.ByUserID = byUserID;
        }

        public int WithdrawID { get; private set; }
        public int ReceiverBankAccountID { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class CNYWithdrawSubmitedToProcess : DomainEvent
    {
        public CNYWithdrawSubmitedToProcess(int cnyWithdrawID, decimal amount, decimal fee, int byUserID)
        {
            this.WithdrawID = cnyWithdrawID;
            this.Amount = amount;
            this.Fee = fee;
            this.ByUserID = byUserID;
        }

        public int WithdrawID { get; private set; }
        public int ByUserID { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
    }

    public class CNYWithdrawCompleted : DomainEvent
    {
        public CNYWithdrawCompleted(int cnyWithdrawID,int withdrawUserID, int transferAccountID, string transferNo, int byUserID)
        {
            this.WithdrawID = cnyWithdrawID;
            this.WithdrawUserID = withdrawUserID;
            this.TransferAccountID = transferAccountID;
            this.TransferNo = transferNo;
            this.ByUserID = byUserID;
        }

        public int WithdrawID { get; private set; }
        public int WithdrawUserID { get; private set; }
        public int TransferAccountID { get; private set; }
        public string TransferNo { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class CNYWithdrawTransferFailed : DomainEvent
    {
        public CNYWithdrawTransferFailed(int cnyWithdrawID, decimal amount, decimal fee, string memo, int byUserID)
        {
            this.WithdrawID = cnyWithdrawID;
            this.Amount = amount;
            this.Fee = fee;
            this.ByUserID = byUserID;
            this.Memo = memo;
        }

        public int WithdrawID { get; private set; }
        public int ByUserID { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
        public string Memo { get; private set; }
    }

    public class CNYWithdrawCanceled : DomainEvent
    {
        public CNYWithdrawCanceled(string cnyWithdrawUniqueID, int cnyAccountID, decimal amount, decimal fee, string memo, int byUserID)
        {
            this.WithdrawUniqueID = cnyWithdrawUniqueID;
            this.Amount = amount;
            this.Fee = fee;
            this.CNYAccountID = cnyAccountID;
            this.ByUserID = byUserID;
            this.Memo = memo;
        }

        public string WithdrawUniqueID { get; private set; }
        public int CNYAccountID { get; private set; }
        public int ByUserID { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
        public string Memo { get; private set; }
    }
}
