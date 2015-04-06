using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Actor.Implementations;
﻿using Dotpay.Actor;
﻿using Dotpay.Actor.Service;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using RabbitMQ.Client;

namespace Dotpay.Actor.Service.Implementations
{
    public class UserResetPasswordService : Orleans.Grain, IUserResetPasswordService
    {
        private const string USER_MQ_EXCHANGE = Constants.UserMQName + Constants.ExechangeSuffix;
        private const string USER_EMAIL_MQ_ROUTE_KEY = Constants.UserEmailRouteKey;
        private const string USER_EMAIL_MQ_QUEUE = Constants.UserEmailMQName + Constants.QueueSuffix;

        async Task IUserResetPasswordService.ForgetLoginPassword(long userId, Lang lang)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            var resetToken = await user.ForgetLoginPassword();
            var userInfo = await user.GetUserInfo();
            var forgetLoginPasswordMsg = new UserForgetLoginPasswordMessage(userInfo.Email, userInfo.LoginName,
                resetToken, DateTime.Now, lang);

            await MessageProducterManager.GetProducter().PublishMessage(forgetLoginPasswordMsg, USER_MQ_EXCHANGE, USER_EMAIL_MQ_ROUTE_KEY, true);
        }

        //此处独立出来，是为了以后在用户重置密码后，可以发邮件给用户
        Task IUserResetPasswordService.ResetLoginPassword(long userId, string newLoginPassword, string resetToken)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            return user.ResetLoginPassword(newLoginPassword, resetToken);
        }

        async Task IUserResetPasswordService.ForgetPaymentPassword(long userId, Lang lang)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            var resetToken = await user.ForgetPaymentPassword();
            var userInfo = await user.GetUserInfo();
            var forgetPaymentPasswordMsg = new UserForgetPaymentPasswordMessage(userInfo.Email, userInfo.LoginName,
                resetToken, DateTime.Now, lang);

            await MessageProducterManager.GetProducter().PublishMessage(forgetPaymentPasswordMsg, USER_MQ_EXCHANGE, USER_EMAIL_MQ_ROUTE_KEY, true);
        }

        //此处独立出来，是为了以后在用户重置支付密码后，可以发邮件给用户
        Task IUserResetPasswordService.ResetPaymentPassword(long userId, string newPaymentPassword, string resetToken)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            return user.ResetLoginPassword(newPaymentPassword, resetToken);
        }

        public override Task OnActivateAsync()
        {
            MessageProducterManager.RegisterAndBindQueue(USER_MQ_EXCHANGE, ExchangeType.Direct, USER_EMAIL_MQ_QUEUE, USER_EMAIL_MQ_ROUTE_KEY, true);
            return base.OnActivateAsync();
        }
    }
}
