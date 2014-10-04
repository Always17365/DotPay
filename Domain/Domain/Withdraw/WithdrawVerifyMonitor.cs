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
using DotPay.Tools.DistributedMessageSender;

namespace DotPay.Domain
{
    public class WithdrawOnceLimitVerifyMonitor : IEventHandler<CNYWithdrawSetFee>,
                                         IEventHandler<VirtualCoinWithdrawSetFee>
    {
        public void Handle(CNYWithdrawSetFee @event)
        {
            var currency = IoC.Resolve<ICurrencyRepository>().FindByCurrencyType(CurrencyType.CNY);

            var needVerify = currency.WithdrawVerifyLine == 0 ? false : currency.WithdrawVerifyLine <= @event.CNYWithdraw.Amount;

            if (!needVerify)
            {
                @event.CNYWithdraw.SkipVerify();
            }
        }

        public void Handle(VirtualCoinWithdrawSetFee @event)
        {
            var currency = IoC.Resolve<ICurrencyRepository>().FindById<Currency>((int)@event.Currency);
            var needVerify = currency.WithdrawVerifyLine == 0 ? false : currency.WithdrawVerifyLine <= @event.VirtualCoinWithdraw.Amount;

            if (!needVerify)
            {
                @event.VirtualCoinWithdraw.SkipVerify();
            }

        }

    }
}
