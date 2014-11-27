using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Repository;
using FC.Framework.Repository;

namespace DotPay.MainDomain
{
    public class WithdrawFeeCalculator : IEventHandler<CNYWithdrawCreated>
                                         //,
                                         //IEventHandler<VirtualCoinWithdrawCreated>
    {
        public void Handle(CNYWithdrawCreated @event)
        {
            var repos = IoC.Resolve<IRepository>();
            var vipsettingRepos = IoC.Resolve<IVipSettingRepository>();
            var currencyRepos = IoC.Resolve<ICurrencyRepository>();

            var currency = currencyRepos.FindByCurrencyType(CurrencyType.CNY);
            var user = repos.FindById<User>(@event.WithdrawUserID);
            var vipsetting = vipsettingRepos.FindByVipLevel(user.VipLevel);

            var fee = currency.WithdrawFixedFee + currency.WithdrawFeeRate * @event.CNYWithdrawEntity.Amount * vipsetting.Discount;

            //if (fee > 0) 体现费无论是否为0，都要触发setfee,避免事件断链
                @event.CNYWithdrawEntity.SetFee(fee);
        }

        //public void Handle(VirtualCoinWithdrawCreated @event)
        //{
        //    var repos = IoC.Resolve<IRepository>();
        //    var vipsettingRepos = IoC.Resolve<IVipSettingRepository>();
        //    var currencyRepos = IoC.Resolve<ICurrencyRepository>();

        //    var currency = currencyRepos.FindByCurrencyType(@event.Currency);
        //    var user = repos.FindById<User>(@event.WithdrawUserID);
        //    var vipsetting = vipsettingRepos.FindByVipLevel(user.VipLevel);

        //    var fee = currency.WithdrawFixedFee + currency.WithdrawFeeRate * @event.WithdrawEntity.Amount * vipsetting.Discount;

        //    //if (fee > 0) 体现费无论是否为0，都要触发setfee,避免事件断链
        //        @event.WithdrawEntity.SetFee(fee);
        //}
    }
}
