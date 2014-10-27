using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;

namespace DotPay.MainDomain
{
    [Component]
    public class LoginLogWriter : IEventHandler<UserLoginSuccess>      //用户登录
    {
        public void Handle(UserLoginSuccess @event)
        {
            var log = new LoginLog(@event.UserID, @event.IP);

            IoC.Resolve<IRepository>().Add(log);
        }
    }
}
