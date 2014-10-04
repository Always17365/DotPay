using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.Domain.Events;
using DotPay.Domain.Repository;
using FC.Framework.Repository;

namespace DotPay.Domain
{
    public class DepositFeeCalculator : IEventHandler<CNYDepositCreated>
    {
        public void Handle(CNYDepositCreated @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var vipsettingRepos = IoC.Resolve<IVipSettingRepository>();
            var currency = IoC.Resolve<ICurrencyRepository>().FindByCurrencyType(CurrencyType.CNY);
            var user = repos.FindById<User>(@event.UserID);
            var vipsetting = vipsettingRepos.FindByVipLevel(user.VipLevel);

            var amount = (@event.DepositSumAmount - currency.DepositFixedFee) / (1 + currency.DepositFeeRate) * vipsetting.Discount;

            var fee = @event.DepositSumAmount - amount;

            @event.CNYDepositEntity.SetAmountAndFee(amount, fee);
        }

        //虚拟币的手续费，会在ConfirmationMonitor中完成计算，无需再次计费
        //public void Handle(VirtualCoinDepositSetFee @event)
        //{
        //    var currency = IoC.Resolve<ICurrencyRepository>().FindByCurrencyType(@event.Currency);

        //    var fee = currency.DepositFixedFee + currency.DepositFeeRate * @event.VirtualCoinDeposit.Amount;

        //    if (fee > 0)
        //        @event.VirtualCoinDeposit.SetFee(@event.Currency, fee);
        //}
    }
}
