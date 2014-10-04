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
    public class TokenProcessor : IEventHandler<TokenGenerated>  //token生成
    {
        public void Handle(TokenGenerated @event)
        {
            var token = new Token(@event.UserID, @event.Token, @event.TokenType, @event.ExpirationTime);

            IoC.Resolve<IRepository>().Add(token);
        }
    }
}
