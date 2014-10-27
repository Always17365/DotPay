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
    public class TokenProcessor : IEventHandler<TokenGenerated>  //token生成
    {
        public void Handle(TokenGenerated @event)
        {
            var token = new Token(@event.UserID, @event.Token, @event.TokenType, @event.ExpirationTime);

            IoC.Resolve<IRepository>().Add(token);
        }
    }
}
