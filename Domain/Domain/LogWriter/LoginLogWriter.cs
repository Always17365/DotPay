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

namespace DotPay.Domain
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
