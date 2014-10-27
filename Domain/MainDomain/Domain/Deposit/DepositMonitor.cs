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
    public class DepositFeeCalculator : IEventHandler<CNYDepositVerified>
    {
        public void Handle(CNYDepositVerified @event)
        {
            var deposit = IoC.Resolve<IRepository>().FindById<CNYDeposit>(@event.DepositID);
            deposit.Complete(@event.ByUserID);
        }
    }
}
