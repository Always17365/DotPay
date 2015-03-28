 

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Actor.Implementations;
﻿using Dotpay.Actor.Interfaces;
﻿using Dotpay.Actor.Service.Interfaces;
﻿using Dotpay.Actor.Tools.Interfaces;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using RabbitMQ.Client;

namespace Dotpay.Actors.Service.Implementations
{

    public class UserRegisterService : Orleans.Grain, IUserRegisterService
    {
        private const string AutoIncreamentKey = "UserId";
        private const string UserMQExchange = Constants.UserMQName + Constants.ExechangeSuffix;
        private const string UserMQQueue = Constants.UserMQName + Constants.QueueSuffix;

        async Task IUserRegisterService.PreRegister(string email, Lang lang)
        {
            var userIdGenerator = GrainFactory.GetGrain<IAtomicIncrement>(AutoIncreamentKey);
            var userId = await userIdGenerator.GetNext();
            var user = GrainFactory.GetGrain<IUser>(userId);
            var errorCode = await user.PreRegister(email);

            if (errorCode == ErrorCode.None)
            {
                var message = new UserPreRegisterMessage(email, lang);
                //发送注册邮件的消息
                await MessageProducterManager.GetProducter().PublishMessage(message, UserMQExchange, "", true);
            }
        }

        async Task IUserRegisterService.InitUserInfo(Int64 userId, string userAccount, string loginPassword, string tradePassword)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            var accountId = await user.Initialize(userAccount, loginPassword, tradePassword);

            var message = new UserInitializedMessage(userId, accountId);
            //初始化用户Account的消息
            await MessageProducterManager.GetProducter().PublishMessage(message, UserMQExchange, "", true);

        }

        public override Task OnActivateAsync()
        {
            MessageProducterManager.RegisterAndBindQueue(UserMQExchange, ExchangeType.Direct, UserMQQueue, durable: true);
            return base.OnActivateAsync();
        }
    }
}
