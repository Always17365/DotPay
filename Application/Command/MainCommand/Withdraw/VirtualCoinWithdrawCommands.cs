using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Withdraw to DepositCode
    public class WithdrawToDepositCode : FC.Framework.Command
    {
        //人民币和虚拟币会共用该命令
        public WithdrawToDepositCode(CurrencyType currency, int withdrawUserID, decimal amount, string tradePassword)
        {
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");
            Check.Argument.IsNotNegativeOrZero(withdrawUserID, "withdrawUserID");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");

            this.Currency = currency;
            this.WithdrawUserID = withdrawUserID;
            this.TradePassword = tradePassword;
            this.Amount = amount;
        }

        public CurrencyType Currency { get; private set; }
        public int WithdrawUserID { get; private set; }
        public decimal Amount { get; private set; }
        public string TradePassword { get; private set; }
        public string CommandResult { get; set; }
    }
    #endregion

    #region Submit VirtualCoin Withdraw
    public class SubmitVirtualCoinWithdraw : FC.Framework.Command
    {
        public SubmitVirtualCoinWithdraw(int withdrawUserID, CurrencyType currency, decimal amount, string receiveAddress, string tradePassword)
        {
            Check.Argument.IsNotNegativeOrZero(withdrawUserID, "withdrawUserID");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");
            Check.Argument.IsNotEmpty(receiveAddress, "receiveAddress");
            Check.Argument.IsNotEmpty(tradePassword, "tradePassword");

            this.WithdrawUserID = withdrawUserID;
            this.Currency = currency;
            this.Amount = amount;
            this.ReceiveAddress = receiveAddress;
            this.TradePassword = tradePassword;
        }

        public int WithdrawUserID { get; private set; }
        public decimal Amount { get; private set; }
        public CurrencyType Currency { get; private set; }
        public string ReceiveAddress { get; private set; }
        public string TradePassword { get; private set; }
    }
    #endregion

    #region VirtualCoin Withdraw Verify
    public class VirtualCoinWithdrawVerify : FC.Framework.Command
    {
        public VirtualCoinWithdrawVerify(int withdrawID, string memo, CurrencyType currency, int verifyBy)
        {
            Check.Argument.IsNotNegativeOrZero(withdrawID, "withdrawID");
            Check.Argument.IsNotNegativeOrZero(verifyBy, "verifyBy");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.WithdrawID = withdrawID;
            this.Memo = memo;
            this.Currency = currency;
            this.ByUserID = verifyBy;
        }

        public int WithdrawID { get; private set; }
        public string Memo { get; private set; }
        public CurrencyType Currency { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Complete VirtualCoin Withdraw
    public class CompleteVirtualCoinWithdraw : FC.Framework.Command
    {
        public CompleteVirtualCoinWithdraw(string withdrawUniqueID, string txid, decimal txfee, CurrencyType currency)
        {
            Check.Argument.IsNotEmpty(withdrawUniqueID, "withdrawUniqueID");
            Check.Argument.IsNotEmpty(txid, "txid");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");
            Check.Argument.IsNotNegative(txfee, "txfee");

            this.WithdrawUniqueID = withdrawUniqueID;
            this.TxID = txid;
            this.TxFee = txfee;
            this.Currency = currency;
        }

        public string WithdrawUniqueID { get; private set; }
        public string TxID { get; private set; }
        public decimal TxFee { get; private set; }
        public CurrencyType Currency { get; private set; }
    }
    #endregion


    #region VirtualCoin Withdraw Fail
    public class VirtualCoinWithdrawFail : FC.Framework.Command
    {
        public VirtualCoinWithdrawFail(string withdrawUniqueID, int byUserID, string memo, CurrencyType currency)
        {
            Check.Argument.IsNotEmpty(withdrawUniqueID, "withdrawUniqueID");
            Check.Argument.IsNotNegative(byUserID, "byUserID");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.WithdrawUniqueID = withdrawUniqueID;
            this.ByUserID = byUserID;
            this.Memo = memo;
            this.Currency = currency;
            this.ProcessType = VirtualCoinWithdrawFailProcessType.MarkFail;
        }

        public string WithdrawUniqueID { get; private set; }
        public int ByUserID { get; private set; }
        public string Memo { get; private set; }
        public CurrencyType Currency { get; private set; }
        public VirtualCoinWithdrawFailProcessType ProcessType { get; set; }
    }
    #endregion

    #region Cancel VirtualCoin Withdraw
    public class CancelVirtualCoinWithdraw : FC.Framework.Command
    {
        public CancelVirtualCoinWithdraw(string withdrawUniqueID, int byUserID, string memo, CurrencyType currency)
        {
            Check.Argument.IsNotEmpty(withdrawUniqueID, "withdrawUniqueID");
            Check.Argument.IsNotNegative(byUserID, "byUserID");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");

            this.WithdrawUniqueID = withdrawUniqueID;
            this.ByUserID = byUserID;
            this.Memo = memo;
            this.Currency = currency;
            this.ProcessType = VirtualCoinWithdrawFailProcessType.Cancel;
        }

        public string WithdrawUniqueID { get; private set; }
        public int ByUserID { get; private set; }
        public string Memo { get; private set; }
        public CurrencyType Currency { get; private set; }
        public VirtualCoinWithdrawFailProcessType ProcessType { get; set; }
    }

    public enum VirtualCoinWithdrawFailProcessType
    {
        MarkFail = 1,
        Cancel = 2
    }
    #endregion
}
