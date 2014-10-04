using FC.Framework;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Events
{
    public class AccountCreated : DomainEvent
    {
        public AccountCreated(int userID)
        {
            this.UserID = userID;
        }
        public int UserID { get; private set; }
    }

    public class AccountBalanceIncrease : DomainEvent
    {
        public AccountBalanceIncrease(int accountID, decimal increase)
        {
            this.AccountID = accountID;
            this.AmountIncrease = increase;
        }
        public int AccountID { get; private set; }
        public decimal AmountIncrease { get; private set; }
    }

    public class AccountBalanceDecrease : DomainEvent
    {
        public AccountBalanceDecrease(int accountID, decimal increase)
        {
            this.AccountID = accountID;
            this.AmountDecrease = increase;
        }
        public int AccountID { get; private set; }
        public decimal AmountDecrease { get; private set; }
    }

    public class AccountLockedIncrease : DomainEvent
    {
        public AccountLockedIncrease(int accountID, decimal increase)
        {
            this.AccountID = accountID;
            this.LockedIncrease = increase;
        }
        public int AccountID { get; private set; }
        public decimal LockedIncrease { get; private set; }
    }

    public class AccountLockedDecrease : DomainEvent
    {
        public AccountLockedDecrease(int accountID, decimal increase)
        {
            this.AccountID = accountID;
            this.LockedDecrease = increase;
        }
        public int AccountID { get; private set; }
        public decimal LockedDecrease { get; private set; }
    }

    public abstract class AccountChanged : DomainEvent
    {
        public int ModifyType { get; protected set; }
    }

    public class AccountChangedByDeposit : AccountChanged
    {
        public AccountChangedByDeposit(int userID, int accountID, decimal depositAmount, int depositID, CurrencyType currency)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.DepositID = depositID;
            this.DepositAmount = depositAmount;
            this.Currency = currency;
            this.ModifyType = Convert.ToInt32(AccountModifyType.Deposit.ToString("D") + currency.ToString("D"));
        }
        public int UserID { get; private set; }
        public int AccountID { get; private set; }
        public int DepositID { get; private set; }
        public decimal DepositAmount { get; private set; }
        public CurrencyType Currency { get; private set; }
    }

    public class AccountChangedByCancelDeposit : AccountChanged
    {
        public AccountChangedByCancelDeposit(int userID, int accountID, decimal depositAmount, int depositID, CurrencyType currency)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.DepositID = depositID;
            this.DepositAmount = depositAmount;
            this.Currency = currency;
            this.ModifyType = Convert.ToInt32(AccountModifyType.Deposit.ToString("D") + currency.ToString("D"));
        }
        public int UserID { get; private set; }
        public int AccountID { get; private set; }
        public int DepositID { get; private set; }
        public decimal DepositAmount { get; private set; }
        public CurrencyType Currency { get; private set; }
    }

    //public class AccountChangedByCreateWithdraw : DomainEvent
    //{
    //    public AccountChangedByCreateWithdraw(int accountID, decimal withdrawAmount, int withdrawID, CurrencyType currency)
    //    {
    //        this.AccountID = accountID;
    //        this.WithdrawID = withdrawID;
    //        this.WithdrawAmount = withdrawAmount;
    //        this.Currency = currency;
    //    }
    //    public int AccountID { get; private set; }
    //    public int WithdrawID { get; private set; }
    //    public decimal WithdrawAmount { get; private set; }
    //    public CurrencyType Currency { get; private set; }
    //}

    public class AccountChangedByWithdrawCreated : AccountChanged
    {
        public AccountChangedByWithdrawCreated(int userID, int accountID, decimal withdrawAmount, string withdarwUniqueId, CurrencyType currency)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.WithdarwUniqueId = withdarwUniqueId;
            this.WithdrawAmount = withdrawAmount;
            this.Currency = currency;
            this.ModifyType = Convert.ToInt32(AccountModifyType.Withdraw.ToString("D") + currency.ToString("D"));
        }
        public int UserID { get; private set; }
        public int AccountID { get; private set; }
        public string WithdarwUniqueId { get; private set; }
        public decimal WithdrawAmount { get; private set; }
        public CurrencyType Currency { get; private set; }
    }

    public class AccountChangedByWithdrawCancel : AccountChanged
    {
        public AccountChangedByWithdrawCancel(int userID, int accountID, decimal withdrawAmount, string withdrawUniqueID, CurrencyType currency)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.WithdrawUniqueID = withdrawUniqueID;
            this.WithdrawAmount = withdrawAmount;
            this.Currency = currency;
            this.ModifyType = Convert.ToInt32(AccountModifyType.Withdraw.ToString("D") + currency.ToString("D"));
        }
        public int UserID { get; private set; }
        public int AccountID { get; private set; }
        public string WithdrawUniqueID { get; private set; }
        public decimal WithdrawAmount { get; private set; }
        public CurrencyType Currency { get; private set; }
    }

    public class AccountChangedByWithdrawToDepositCode : AccountChanged
    {
        public AccountChangedByWithdrawToDepositCode(int userID, int accountID, decimal withdrawAmount, string depositCodeUniqueID, CurrencyType currency)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.DepositCodeUniqueID = depositCodeUniqueID;
            this.WithdrawAmount = withdrawAmount;
            this.Currency = currency;
            this.ModifyType = Convert.ToInt32(AccountModifyType.Withdraw.ToString("D") + currency.ToString("D"));
        }
        public int UserID { get; private set; }
        public int AccountID { get; private set; }
        public string DepositCodeUniqueID { get; private set; }
        public decimal WithdrawAmount { get; private set; }
        public CurrencyType Currency { get; private set; }
    }

    //public class AccountChangedByOrderCreated : DomainEvent
    //{
    //    public AccountChangedByOrderCreated(int accountID, int orderID, CurrencyType currency)
    //    {
    //        this.AccountID = accountID;
    //        this.OrderID = orderID;
    //        this.Currency = currency;
    //    }
    //    public int AccountID { get; private set; }
    //    public int OrderID { get; private set; }
    //    public CurrencyType Currency { get; private set; }
    //}

    //public class AccountChangedByOrderCanceled : DomainEvent
    //{
    //    public AccountChangedByOrderCanceled(int accountID, int orderID, CurrencyType currency)
    //    {
    //        this.AccountID = accountID;
    //        this.OrderID = orderID;
    //        this.Currency = currency;
    //    }
    //    public int AccountID { get; private set; }
    //    public int OrderID { get; private set; }
    //    public CurrencyType Currency { get; private set; }
    //}

    public class AccountChangedByTrade : AccountChanged
    {
        public AccountChangedByTrade(int accountID, decimal @in, decimal @out, int tradeID, CurrencyType currency)
        {
            this.AccountID = accountID;
            this.TradeID = tradeID;
            this.In = @in;
            this.Out = @out;
            this.Currency = currency;
            this.ModifyType = Convert.ToInt32(AccountModifyType.Trade.ToString("D") + currency.ToString("D"));
        }
        public int AccountID { get; private set; }
        public int TradeID { get; private set; }
        public decimal In { get; private set; }
        public decimal Out { get; private set; }
        public CurrencyType Currency { get; private set; }
    }


    public class AccountVersionCreated : DomainEvent
    {
        public AccountVersionCreated(int userID, int accountID, decimal amount,
                              decimal balance, decimal locked, decimal @in, decimal @out,
                              int modifyID, int modifyType)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.Amount = amount;
            this.Balance = balance;
            this.Locked = locked;
            this.AmountIn = @in;
            this.AmountOut = @out;
            this.ModifyID = modifyID;
            this.ModifyType = modifyType;
            this.ModifyUniqueID = string.Empty;
        }
        public AccountVersionCreated(int userID, int accountID, decimal amount,
                             decimal balance, decimal locked, decimal @in, decimal @out,
                             string depositCode, int modifyType)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.Amount = amount;
            this.Balance = balance;
            this.Locked = locked;
            this.AmountIn = @in;
            this.AmountOut = @out;
            this.ModifyUniqueID = depositCode;
            this.ModifyID = 0;
            this.ModifyType = modifyType;
        }
        public int AccountID { get; private set; }
        public int UserID { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Balance { get; private set; }
        public decimal Locked { get; private set; }
        public decimal AmountIn { get; private set; }
        public decimal AmountOut { get; private set; }
        public int ModifyID { get; private set; }
        public string ModifyUniqueID { get; private set; }
        public int ModifyType { get; private set; }
    }
}
