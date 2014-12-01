using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using DotPay.Common;
using FC.Framework;
using DotPay.MainDomain.Events;
using FC.Framework.Repository;
using DotPay.MainDomain.Repository;

namespace DotPay.MainDomain
{
    public class AccountVersionMonitor : IEventHandler<AccountChangedByDeposit>,                //充值成功
                                         IEventHandler<AccountChangedByCancelDeposit>,          //撤销充值
                                         IEventHandler<AccountChangedByWithdrawCreated>,        //提现成功
                                         IEventHandler<AccountChangedByWithdrawToDepositCode>,  //提现到充值码
                                         IEventHandler<AccountChangedByWithdrawCancel>,         //取消提现
        //IEventHandler<AccountChangedByOrderCanceled>,          //订单取消
                                         IEventHandler<AccountChangedByInsideTransfer>                   //交易
    {
        IRepository repos = IoC.Resolve<IRepository>();

        public void Handle(AccountChangedByDeposit @event)
        {
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

            var accountVersion = AccountVersionFactory.CreateAccountVersion(account.UserID, @event.AccountID,
                                                    account.Balance + account.Locked, account.Balance,
                                                    account.Locked, @event.DepositAmount, 0, @event.DepositID,
                                                    @event.ModifyType, @event.Currency);
            repos.Add(accountVersion);
        }
        public void Handle(AccountChangedByCancelDeposit @event)
        {
            var accountVersionRepos = IoC.Resolve<IAccountVersionRepository>();
            var accountVersion = accountVersionRepos.FindByDepositIDAndCurrency(@event.DepositID, @event.Currency);

            accountVersionRepos.Remove(accountVersion);
        } 
        //public void Handle(AccountChangedByCreateWithdraw @event)
        //{
        //    var account = repos.FindById<Account>(@event.AccountID);

        //    var accountVersion = AccountVersionFactory.CreateAccountVersion(account.UserID, @event.AccountID,
        //                                  account.Balance + account.Locked, account.Balance,
        //                                  account.Locked, 0, @event.WithdrawAmount, @event.WithdrawID,
        //                                  AccountModifyType.Withdraw, @event.Currency);
        //    repos.Add(accountVersion);
        //}

        public void Handle(AccountChangedByWithdrawCreated @event)
        {
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

            var accountVersion = AccountVersionFactory.CreateAccountVersion(account.UserID, @event.AccountID,
                                                    account.Balance + account.Locked, account.Balance,
                                                    account.Locked, 0, @event.WithdrawAmount, @event.WithdarwUniqueId,
                                                    @event.ModifyType, @event.Currency);
            repos.Add(accountVersion);
        }

        public void Handle(AccountChangedByInsideTransfer @event)
        {
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

            var accountVersion = AccountVersionFactory.CreateAccountVersion(account.UserID, @event.AccountID,
                                                    account.Balance + account.Locked, account.Balance,
                                                    account.Locked, @event.In, @event.Out, @event.TradeID,
                                                    @event.ModifyType, @event.Currency);
            repos.Add(accountVersion);
        }


        //public void Handle(AccountChangedByOrderCreated @event)
        //{
        //    var account = repos.FindById<Account>(@event.AccountID);

        //    var accountVersion = AccountVersionFactory.CreateAccountVersion(account.UserID, @event.AccountID,
        //                                            account.Balance + account.Locked, account.Balance,
        //                                            account.Locked, 0, 0, @event.OrderID,
        //                                            AccountModifyType.Withdraw, @event.Currency);
        //    repos.Add(accountVersion);
        //}

        //public void Handle(AccountChangedByOrderCanceled @event)
        //{
        //    var account = repos.FindById<Account>(@event.AccountID);

        //    var accountVersion = AccountVersionFactory.CreateAccountVersion(account.UserID, @event.AccountID,
        //                                            account.Balance + account.Locked, account.Balance,
        //                                            account.Locked, 0, 0, @event.OrderID,
        //                                            AccountModifyType.Withdraw, @event.Currency);
        //    repos.Add(accountVersion);
        //}

        public void Handle(AccountChangedByWithdrawToDepositCode @event)
        {
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

            var accountVersion = AccountVersionFactory.CreateAccountVersion(account.UserID, @event.AccountID,
                                                    account.Balance + account.Locked, account.Balance,
                                                    account.Locked, 0, @event.WithdrawAmount, @event.DepositCodeUniqueID,
                                                    @event.ModifyType, @event.Currency);
            repos.Add(accountVersion);
        }

        public void Handle(AccountChangedByWithdrawCancel @event)
        {
            var accountVersionRepos = IoC.Resolve<IAccountVersionRepository>();

            var accountVersion = accountVersionRepos.FindByWithdrawIDAndCurrency(@event.WithdrawUniqueID, @event.Currency); 
            accountVersionRepos.Remove(accountVersion);
        }
    }
}
