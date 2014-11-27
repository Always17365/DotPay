using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Submit CNY Withdraw
    public class SubmitCNYWithdraw : FC.Framework.Command
    {
        public SubmitCNYWithdraw(int? receiverBankAccountID, int withdrawUserID, decimal amount,
                                 PayWay payway, string receiverBankAccount, string tradePassword)
        {
            Check.Argument.IsNotNegativeOrZero(withdrawUserID, "withdrawUserID");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");
            Check.Argument.IsNotEmpty(tradePassword, "tradePassword");


            this.WithdrawUserID = withdrawUserID;
            this.Amount = amount;
            this.TradePassword = tradePassword;

            if (receiverBankAccountID.HasValue)
            {
                Check.Argument.IsNotNegativeOrZero(receiverBankAccountID.Value, "receiverBankAccountID");

                this.ReceiverBankAccountID = receiverBankAccountID;
            }
            else
            { 
                Check.Argument.IsNotEmpty(receiverBankAccount, "receiverBankAccount");
                this.PayWay = payway;
                this.BankAccount = receiverBankAccount;
            }
        }

        public int WithdrawUserID { get; private set; }
        public decimal Amount { get; private set; }
        public int? ReceiverBankAccountID { get; private set; }
        public PayWay PayWay { get; private set; }
        public bool Valid { get; private set; }
        public string BankAccount { get; private set; }
        public string TradePassword { get; private set; }
    }
    #endregion

    #region CNY Withdraw Verify
    public class CNYWithdrawVerify : FC.Framework.Command
    {
        public CNYWithdrawVerify(int withdrawID, string memo, int verifyBy)
        {
            Check.Argument.IsNotNegativeOrZero(withdrawID, "withdrawID");
            Check.Argument.IsNotNegativeOrZero(verifyBy, "verifyBy");

            this.WithdrawID = withdrawID;
            this.Memo = memo;
            this.ByUserID = verifyBy;
        }

        public int WithdrawID { get; private set; }
        public string Memo { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region CNY Withdraw Submit To Process
    public class SubmitCNYWithdrawToProcess : FC.Framework.Command
    {
        public SubmitCNYWithdrawToProcess(int withdrawID, int submitBy)
        {
            Check.Argument.IsNotNegativeOrZero(withdrawID, "withdrawID");
            Check.Argument.IsNotNegativeOrZero(submitBy, "submitBy");


            this.WithdrawID = withdrawID;
            this.ByUserID = submitBy;
        }

        public int WithdrawID { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region CNY Withdraw Mark As Success
    public class CNYWithdrawMarkAsSuccess : FC.Framework.Command
    {
        public CNYWithdrawMarkAsSuccess(int withdrawID, int transferAccountID, string transferNo, int markBy)
        {
            Check.Argument.IsNotNegativeOrZero(withdrawID, "withdrawID");
            Check.Argument.IsNotNegativeOrZero(transferAccountID, "transferAccountID");
            Check.Argument.IsNotEmpty(transferNo, "transferNo");
            Check.Argument.IsNotNegativeOrZero(markBy, "markBy");

            this.WithdrawID = withdrawID;
            this.TransferAccountID = transferAccountID;
            this.TransferNo = transferNo;
            this.ByUserID = markBy;
        }

        public int WithdrawID { get; private set; }
        public int TransferAccountID { get; private set; }
        public string TransferNo { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region CNY Withdraw Mark As Transfer  Fail
    public class CNYWithdrawMarkAsTransferFail : FC.Framework.Command
    {
        public CNYWithdrawMarkAsTransferFail(int withdrawID, string memo, int verifyBy)
        {
            Check.Argument.IsNotNegativeOrZero(withdrawID, "withdrawID");
            Check.Argument.IsNotNegativeOrZero(verifyBy, "verifyBy");

            this.WithdrawID = withdrawID;
            this.Memo = memo;
            this.ByUserID = verifyBy;
        }

        public int WithdrawID { get; private set; }
        public string Memo { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region CNY Withdraw Modify Recever Bank Account
    public class CNYWithdrawModifyReceiverBankAccount : FC.Framework.Command
    {
        public CNYWithdrawModifyReceiverBankAccount(int withdrawID, PayWay payway, string bankAccount, int modifyBy)
        {
            Check.Argument.IsNotNegativeOrZero(withdrawID, "withdrawID");
            Check.Argument.IsNotEmpty(bankAccount, "bankAccount");
            Check.Argument.IsNotNegativeOrZero(modifyBy, "modifyBy");

            this.WithdrawID = withdrawID;
            this.PayWay = payway;
            this.BankAccount = bankAccount;
            this.ByUserID = modifyBy;
        }

        public int WithdrawID { get; private set; }
        public PayWay PayWay { get; private set; }
        public int BankID { get; private set; }
        public string BankAccount { get; private set; }
        public string BankAddress { get; private set; }
        public string OpenBankName { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region CNY Withdraw Cannel
    public class CNYWithdrawCancel : FC.Framework.Command
    {
        public CNYWithdrawCancel(int withdrawID, string memo, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(withdrawID, "withdrawID");
            Check.Argument.IsNotEmpty(memo, "memo");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.WithdrawID = withdrawID;
            this.Memo = memo;
            this.ByUserID = byUserID;
        }

        public int WithdrawID { get; private set; }
        public string Memo { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion
}
