using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Exceptions;
using FC.Framework.Repository;
using DotPay.Common;
using DotPay.MainDomain.Repository;

namespace DotPay.MainDomain
{
    public class AccountMonitor : IEventHandler<CNYDepositCompleted>,                 //cny充值完成
                                  IEventHandler<CNYDepositUndoComplete>,              //撤销cny充值
                                  IEventHandler<VirtualCoinDepositCompleted>,         //虚拟币充值完成 
                                  IEventHandler<CNYWithdrawCreated>,                  //cny提现创建 
                                  IEventHandler<CNYWithdrawSetFee>,                   //cny提现计入提现费
        //IEventHandler<CNYWithdrawCompleted>,                //cny提现完成
                                  IEventHandler<CNYWithdrawCanceled>,                 //cny提现撤销
                                  IEventHandler<VirtualCoinWithdrawCreated>,          //虚拟币提现创建
                                  IEventHandler<VirtualCoinWithdrawSetFee>,           //虚拟币计入提现费
                                  IEventHandler<VirtualCoinWithdrawCanceled>          //cny提现撤销 
    {
        private IRepository repos = IoC.Resolve<IRepository>();

        public void Handle(CNYDepositCompleted @event)
        {
            var account = repos.FindById<CNYAccount>(@event.AccountID);

            account.BalanceIncrease(@event.DepositAmount);

            this.Apply(new AccountChangedByDeposit(@event.DepositUserID, @event.AccountID, @event.DepositAmount,
                                                   @event.DepositID, CurrencyType.CNY));
        }

        public void Handle(CNYDepositUndoComplete @event)
        {
            var account = repos.FindById<CNYAccount>(@event.AccountID);

            if (account.Balance < @event.DepositAmount)
                throw new DepositUndoCompleteForCNYException();

            account.BalanceDecrease(@event.DepositAmount);

            this.Apply(new AccountChangedByCancelDeposit(account.UserID, @event.AccountID, @event.DepositAmount,
                                                         @event.DepositID, CurrencyType.CNY));
        }

        public void Handle(VirtualCoinDepositCompleted @event)
        {
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

            account.BalanceIncrease(@event.DepositAmount);

            this.Apply(new AccountChangedByDeposit(account.UserID, @event.AccountID, @event.DepositAmount,
                                                   @event.DepositID, @event.Currency));
        }


        public void Handle(CNYWithdrawCreated @event)
        {
            var account = repos.FindById<CNYAccount>(@event.AccountID);

            account.BalanceDecrease(@event.Amount);

            this.Apply(new AccountChangedByWithdrawCreated(account.UserID, @event.AccountID, @event.Amount,
                                                           @event.CNYWithdrawEntity.UniqueID, CurrencyType.CNY));
        }

        public void Handle(CNYWithdrawSetFee @event)
        {
            var account = repos.FindById<CNYAccount>(@event.CNYWithdraw.AccountID);

            account.BalanceDecrease(@event.CNYWithdraw.Fee);
            //account.LockedIncrease(@event.CNYWithdraw.Fee);
        }

        public void Handle(VirtualCoinWithdrawSetFee @event)
        {
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

            account.BalanceDecrease(@event.VirtualCoinWithdraw.Fee);
        }

        //public void Handle(CNYWithdrawCompleted @event)
        //{
        //    var cnyWithdraw = repos.FindById<CNYWithdraw>(@event.WithdrawID);
        //    var account = repos.FindById<Account>(cnyWithdraw.AccountID);

        //    var withdrawSum = cnyWithdraw.Amount + cnyWithdraw.Fee;
        //    account.LockedDecrease(withdrawSum);

        //    this.Apply(new AccountChangedByWithdrawComplete(cnyWithdraw.AccountID, withdrawSum, @event.WithdrawID, CurrencyType.CNY));
        //} 

        public void Handle(CNYWithdrawCanceled @event)
        {
            var cnyWithdraw = IoC.Resolve<IWithdrawRepository>().FindByUniqueIdAndCurrency(@event.WithdrawUniqueID, CurrencyType.CNY);
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.CNYAccountID, CurrencyType.CNY);

            var withdrawSum = cnyWithdraw.Amount + cnyWithdraw.Fee;
            //account.LockedDecrease(withdrawSum);
            account.BalanceIncrease(withdrawSum);

            this.Apply(new AccountChangedByWithdrawCancel(cnyWithdraw.UserID, cnyWithdraw.AccountID, withdrawSum, @event.WithdrawUniqueID, CurrencyType.CNY));
        }

        public void Handle(VirtualCoinWithdrawCanceled @event)
        {
            var withdraw = IoC.Resolve<IWithdrawRepository>().FindByUniqueIdAndCurrency(@event.WithdrawUniqueID, @event.Currency);
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

            var withdrawSum = withdraw.Amount + withdraw.Fee;
            //account.LockedDecrease(withdrawSum);
            account.BalanceIncrease(withdrawSum);

            this.Apply(new AccountChangedByWithdrawCancel(withdraw.UserID, withdraw.AccountID, withdrawSum, @event.WithdrawUniqueID, @event.Currency));
        }

        public void Handle(VirtualCoinWithdrawCreated @event)
        {
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

            account.BalanceDecrease(@event.Amount);
            //account.LockedIncrease(@event.Amount); 

            this.Apply(new AccountChangedByWithdrawCreated(account.UserID, @event.AccountID, @event.Amount,
                                                           @event.WithdrawEntity.UniqueID, @event.Currency));
        }

    }
}
