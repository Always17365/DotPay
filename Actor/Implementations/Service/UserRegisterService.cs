 ﻿using System.Threading.Tasks;
﻿using Dotpay.Actor;
﻿using Dotpay.Actor.Implementations;
﻿using Dotpay.Actor.Service;
﻿using Dotpay.Actor.Tools;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using RabbitMQ.Client;

namespace Dotpay.Actors.Service.Implementations
{

    public class UserRegisterService : Grain, IUserRegisterService
    {
        private const string AUTO_INCREAMENT_KEY = "UserId";
        private const string USER_MQ_EXCHANGE = Constants.UserMQName + Constants.ExechangeSuffix;
        private const string USER_MQ_ROUTE_KEY = Constants.UserRouteKey;
        private const string USER_MQ_QUEUE = Constants.UserMQName + Constants.QueueSuffix;
        private const string USER_EMAIL_MQ_ROUTE_KEY = Constants.UserEmailRouteKey;
        private const string USER_EMAIL_MQ_QUEUE = Constants.UserEmailMQName + Constants.QueueSuffix;

        async Task IUserRegisterService.PreRegister(string email, Lang lang)
        {
            var userIdGenerator = GrainFactory.GetGrain<IAtomicIncrement>(AUTO_INCREAMENT_KEY);
            var userId = await userIdGenerator.GetNext();
            var user = GrainFactory.GetGrain<IUser>(userId);
            var errorCode = await user.PreRegister(email);

            if (errorCode == ErrorCode.None)
            {
                var message = new UserPreRegisterMessage(email, lang);
                //发送注册邮件的消息
                await MessageProducterManager.GetProducter().PublishMessage(message, USER_MQ_EXCHANGE, USER_EMAIL_MQ_ROUTE_KEY, true);
            }
        }

        async Task IUserRegisterService.InitUserInfo(long userId, string userAccount, string loginPassword, string tradePassword)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            var accountId = await user.Initialize(userAccount, loginPassword, tradePassword);

            var message = new UserInitializedMessage(userId, accountId);
            //初始化用户Account的消息
            await MessageProducterManager.GetProducter().PublishMessage(message, USER_MQ_EXCHANGE, USER_MQ_ROUTE_KEY, true);

        }

        public override Task OnActivateAsync()
        {
            MessageProducterManager.RegisterAndBindQueue(USER_MQ_EXCHANGE, ExchangeType.Direct, USER_MQ_QUEUE, USER_MQ_ROUTE_KEY, true);
            MessageProducterManager.RegisterAndBindQueue(USER_MQ_EXCHANGE, ExchangeType.Direct, USER_EMAIL_MQ_QUEUE, USER_EMAIL_MQ_ROUTE_KEY, true);
            return base.OnActivateAsync();
        }
    }
}
