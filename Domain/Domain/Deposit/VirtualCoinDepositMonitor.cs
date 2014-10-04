using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.Domain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.Domain.Exceptions;
using DotPay.Domain.Repository;
using DotPay.Tools.DistributedMessageSender;

namespace DotPay.Domain
{
    public class VirtualCoinDepositMonitor : IEventHandler<PaymentTransactionCreated>
    {
        public void Handle(PaymentTransactionCreated @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var accountRepos = IoC.Resolve<IAccountRepository>();
            var addressRepos = IoC.Resolve<IPaymentAddressRepository>();
            var receivePaymentRepos = IoC.Resolve<IReceivePaymentTransactionRepository>();
            var vipsettingRepos = IoC.Resolve<IVipSettingRepository>();
            var currencyID = (int)@event.Currency;

            var currency = repos.FindById<Currency>(currencyID);
            var paymentAddress = @event.Currency == CurrencyType.STR ? null : addressRepos.FindByAddressAndCurrency(@event.Address, @event.Currency);

            if (paymentAddress == null && @event.Currency != CurrencyType.STR) return;
            else
            {
                var account = default(Account);
                var user = default(User);

                if (paymentAddress != null)
                {
                    account = accountRepos.FindByIDAndCurrency(paymentAddress.AccountID, @event.Currency);
                    user = repos.FindById<User>(paymentAddress.UserID);
                }
                else
                {
                    account = accountRepos.FindByUserIDAndCurrency(@event.UserID, @event.Currency);
                    user = repos.FindById<User>(@event.UserID);
                }
                var vipsetting = vipsettingRepos.FindByVipLevel(user.VipLevel); 

                var depositFee = currency.DepositFixedFee + currency.DepositFeeRate * @event.Amount * vipsetting.Discount;
                var depositAmount = @event.Amount - depositFee;

                var deposit = VirtualCoinDepositFactory.CreateDeposit(account.UserID, account.ID,
                                          @event.TxID, @event.Currency, depositAmount, depositFee,
                                          string.Empty, DepositType.Automatic,@event.PtxEntity.UniqueID);

                repos.Add(deposit);
            }
        }
    }
    [Component]
    [AwaitCommitted]
    public class LargeVirtualCoinDepositMonitor : IEventHandler<PaymentTransactionCreated>
    {
        public void Handle(PaymentTransactionCreated @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var addressRepos = IoC.Resolve<IPaymentAddressRepository>();

            var paymentAddress = @event.Currency == CurrencyType.STR ? null : addressRepos.FindByAddressAndCurrency(@event.Address, @event.Currency);

            if (paymentAddress == null && @event.Currency == CurrencyType.STR) return;
            else
            {
                var user = default(User);

                if (paymentAddress != null)
                {
                    user = repos.FindById<User>(paymentAddress.UserID);
                }
                else
                {
                    user = repos.FindById<User>(@event.UserID);
                }

                var exchangeName = Utilities.GenerateVirtualCoinDepositExchangeAndQueueName().Item1;
                var msg = new VirtualCoinDepositMessage
                {
                    UserID = user.ID,
                    Email = user.Email,
                    NickName = user.NickName,
                    Amount = @event.Amount,
                    Currency = @event.Currency
                };
                var msgBytes = IoC.Resolve<IJsonSerializer>().Serialize(msg);
                MessageSender.Send(exchangeName, Encoding.UTF8.GetBytes(msgBytes), true);
            }
        }

        private class VirtualCoinDepositMessage
        {
            public int UserID { get; set; }
            public string Email { get; set; }
            public string NickName { get; set; }
            public decimal Amount { get; set; }
            public CurrencyType Currency { get; set; }
        }
    }
}