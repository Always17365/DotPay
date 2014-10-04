using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.Domain.Events;
using FC.Framework.Repository;
using DotPay.Domain.Repository;

namespace DotPay.Domain
{
    public class ConfirmationMonitor : IEventHandler<PaymentTransactionConfirmed>
    {
        public void Handle(PaymentTransactionConfirmed @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var addressRepos = IoC.Resolve<IPaymentAddressRepository>();
            var receivePaymentRepos = IoC.Resolve<IReceivePaymentTransactionRepository>();
            var currencyID = (int)@event.Currency;

            var currency = repos.FindById<Currency>(currencyID);
            var paymentTx = receivePaymentRepos.FindByIDAndCurrency(@event.PaymentTransactionID, @event.Currency);
            var paymentAddress = @event.Currency == CurrencyType.STR ? null : addressRepos.FindByAddressAndCurrency(paymentTx.Address, @event.Currency);


            //如果充值地址不存在，这样的自动充值将被标记为完成，不在重复处理
            if (paymentAddress == null && @event.Currency != CurrencyType.STR)
                return;
            else
            {
                //创建充值，并自动核实、确认 
                var deposit = default(VirtualCoinDeposit);

                if (paymentAddress != null)
                    deposit = IoC.Resolve<IDepositRepository>().FindByTxIDAndCurrency(paymentTx.TxID, @event.Currency);
                else if (@event.Currency == CurrencyType.STR)
                    deposit = IoC.Resolve<IDepositRepository>().FindByReceivePaymentTxUniqueIDAndCurrency(paymentTx.UniqueID, @event.Currency);

                if (currency.NeedConfirm <= paymentTx.Confirmation)
                {
                    deposit.VerifyAmount(@event.ByUserID, paymentTx.TxID, paymentTx.Amount);
                    deposit.Complete(@event.Currency, @event.ByUserID);
                }
            }
        }
    }
}
