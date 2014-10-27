using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.MainDomain.Events;
using FC.Framework.Repository;
using DotPay.MainDomain.Repository;

namespace DotPay.MainDomain
{
    public class FundSourceMonitor : IEventHandler<CNYDepositUndoComplete>    //cny充值撤销完成
    { 
        public void Handle(CNYDepositUndoComplete @event)
        {
            var cnyDeposit = IoC.Resolve<IRepository>().FindById<CNYDeposit>(@event.DepositID);
            var fundSource = IoC.Resolve<IFundSourceRepository>().FindByDepositID(@event.DepositID);

            fundSource.Destroy(@event.ByUserID);
        }
    }
}
