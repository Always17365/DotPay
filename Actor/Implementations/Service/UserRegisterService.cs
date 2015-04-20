 ﻿using System;
 ﻿using System.Globalization;
 ﻿using System.Threading.Tasks;
﻿using Dotpay.Actor;
﻿using Dotpay.Actor.Implementations;
﻿using Dotpay.Actor.Service;
﻿using Dotpay.Actor.Tools;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
 ﻿using Orleans.Concurrency;
 ﻿using RabbitMQ.Client;
using System.Security.Cryptography;
 ﻿using System.Text;

namespace Dotpay.Actors.Service.Implementations
{
    [StatelessWorker]
    public class UserRegisterService : Grain, IUserRegisterService
    {
        private const string USER_MQ_EXCHANGE = Constants.UserMQName + Constants.ExechangeSuffix;
        private const string USER_MQ_ROUTE_KEY = Constants.UserRouteKey;
        private const string USER_MQ_QUEUE = Constants.UserMQName + Constants.QueueSuffix;
        private const string USER_EMAIL_MQ_ROUTE_KEY = Constants.UserEmailRouteKey;
        private const string USER_EMAIL_MQ_QUEUE = Constants.UserEmailMQName + Constants.QueueSuffix;

        async Task IUserRegisterService.Register(string loginName, string email, string loginPassword, Lang lang)
        {
            var userId = Guid.NewGuid();
            email = email.ToLower();
            loginName = loginName.ToLower();
            var user = GrainFactory.GetGrain<IUser>(userId);
            var token = this.GenerteEmailValidateToken(email);
            var errorCode = await user.Register(email, loginName, loginPassword, lang, token);

            if (errorCode == ErrorCode.None)
            {
                var message = new UserRegisterMessage(email, loginName, lang, token);
                //发送注册邮件的消息
                await MessageProducterManager.GetProducter().PublishMessage(message, USER_MQ_EXCHANGE, USER_EMAIL_MQ_ROUTE_KEY, true);
            }
        }

        async Task<ErrorCode> IUserRegisterService.ResendActiveEmail(Guid userId)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            var userInfo = await user.GetUserInfo();
            var token = this.GenerteEmailValidateToken(userInfo.Email);
            var errorCode = await user.ResetActiveToken(token);
            if (errorCode == ErrorCode.None)
            {
                var message = new UserRegisterMessage(userInfo.Email, userInfo.LoginName, userInfo.Lang, token);
                //发送注册邮件的消息
                await MessageProducterManager.GetProducter().PublishMessage(message, USER_MQ_EXCHANGE, USER_EMAIL_MQ_ROUTE_KEY, true);
            }

            return errorCode;
        }

        async Task<ErrorCode> IUserRegisterService.ActiveUser(Guid userId, string token)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            var errorCode = await user.Active(token);

            if (errorCode == ErrorCode.None)
            {
                var accountId = Guid.NewGuid();
                var message = new UserActivedMessage(userId, accountId);
                await MessageProducterManager.GetProducter().PublishMessage(message, USER_MQ_EXCHANGE, USER_EMAIL_MQ_ROUTE_KEY, true);
            }

            return errorCode;
        }
        public override Task OnActivateAsync()
        {
            MessageProducterManager.RegisterAndBindQueue(USER_MQ_EXCHANGE, ExchangeType.Direct, USER_MQ_QUEUE, USER_MQ_ROUTE_KEY, true);
            MessageProducterManager.RegisterAndBindQueue(USER_MQ_EXCHANGE, ExchangeType.Direct, USER_EMAIL_MQ_QUEUE, USER_EMAIL_MQ_ROUTE_KEY, true);
            return base.OnActivateAsync();
        }
        #region Private method
        private string GenerteEmailValidateToken(string email)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            var targetBytes = Encoding.UTF8.GetBytes(email + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            var md5Bytes = md5.ComputeHash(targetBytes);

            var result = BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();

            return result;
        }
        #endregion
    }
}
