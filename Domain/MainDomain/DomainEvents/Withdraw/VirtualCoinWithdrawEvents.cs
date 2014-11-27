using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
namespace DotPay.MainDomain.Events
{
    public class VirtualCoinWithdrawCreated : DomainEvent
    {
        public VirtualCoinWithdrawCreated(int withdrawUserID, int accountID, decimal amount,
                                          string receiveAddress, CurrencyType currency,
                                          VirtualCoinWithdraw withdraw)
        {
            this.WithdrawUserID = withdrawUserID;
            this.AccountID = accountID;
            this.Amount = amount;
            this.ReceiveAddress = receiveAddress;
            this.Currency = currency;
            this.Currency = currency;
            this.WithdrawEntity = withdraw;
        }

        public int WithdrawUserID { get; private set; }
        public int AccountID { get; private set; }
        public decimal Amount { get; private set; }
        public string ReceiveAddress { get; private set; }
        public CurrencyType Currency { get; private set; }
        public VirtualCoinWithdraw WithdrawEntity { get; private set; }
    }

    public class VirtualCoinWithdrawSetFee : DomainEvent
    {
        public VirtualCoinWithdrawSetFee(VirtualCoinWithdraw withdraw, int accountID, CurrencyType currency, decimal fee)
        {
            this.VirtualCoinWithdraw = withdraw;
            this.AccountID = accountID;
            this.Currency = currency;
            this.Fee = fee;
        }

        public int AccountID { get; private set; }
        public decimal Fee { get; private set; }
        public CurrencyType Currency { get; private set; }
        public VirtualCoinWithdraw VirtualCoinWithdraw { get; private set; }
    }

    public class VirtualCoinWithdrawVerified : DomainEvent
    {
        public VirtualCoinWithdrawVerified(CurrencyType currency, int virtualCoinWithdrawID, string virtualCoinWithdrawUniqueID, decimal amount, decimal fee, string memo, int byUserID)
        {
            this.Currency = currency;
            this.WithdrawID = virtualCoinWithdrawID;
            this.WithdrawUniqueID = virtualCoinWithdrawUniqueID;
            this.Amount = amount;
            this.Memo = memo;
            this.Fee = fee;
            this.ByUserID = byUserID;
        }

        public CurrencyType Currency { get; private set; }
        public int WithdrawID { get; private set; }
        public string WithdrawUniqueID { get; private set; }
        public int ByUserID { get; private set; }
        public string Memo { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
    }

    public class VirtualCoinWithdrawSkipVerify : DomainEvent
    {
        public VirtualCoinWithdrawSkipVerify(CurrencyType currency, int virtualCoinWithdrawID,
                string virtualCoinWithdrawUniqueID, VirtualCoinWithdraw withdrawEntity)
        {
            this.Currency = currency;
            this.WithdrawID = virtualCoinWithdrawID;
            this.WithdrawUniqueID = virtualCoinWithdrawUniqueID;
            this.WithdrawEntity = withdrawEntity;
        }

        public CurrencyType Currency { get; private set; }
        public int WithdrawID { get; private set; }
        public string WithdrawUniqueID { get; private set; }
        public VirtualCoinWithdraw WithdrawEntity { get; private set; }
    }

    public class VirtualCoinWithdrawCompleted : DomainEvent
    {
        public VirtualCoinWithdrawCompleted(int virtualCoinWithdrawID, int withdrawUserID, string txid, decimal txfee, CurrencyType currency)
        {
            this.WithdrawID = virtualCoinWithdrawID;
            this.WithdrawUserID = withdrawUserID;
            this.TxID = txid;
            this.TxFee = txfee;
            this.Currency = currency;
        }

        public int WithdrawID { get; private set; }
        public int WithdrawUserID { get; private set; }
        public string TxID { get; private set; }
        public decimal TxFee { get; private set; }
        public CurrencyType Currency { get; private set; }
    }

    public class VirtualCoinWithdrawTranferFailed : DomainEvent
    {
        public VirtualCoinWithdrawTranferFailed(int virtualCoinWithdrawID, decimal amount, decimal fee, string memo, int byUserID)
        {
            this.WithdrawID = virtualCoinWithdrawID;
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

    public class VirtualCoinWithdrawCanceled : DomainEvent
    {
        public VirtualCoinWithdrawCanceled(string withdrawUniqueID, CurrencyType currency, int accountID, decimal amount, decimal fee, string memo, int byUserID)
        {
            this.WithdrawUniqueID = withdrawUniqueID;
            this.Currency = currency;
            this.AccountID = accountID;
            this.Amount = amount;
            this.Fee = fee;
            this.ByUserID = byUserID;
            this.Memo = memo;
        }

        public string WithdrawUniqueID { get; private set; }
        public CurrencyType Currency { get; private set; }
        public int AccountID { get; private set; }
        public int ByUserID { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
        public string Memo { get; private set; }
    }
}
*/