using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Actor.Implementations;
﻿using Dotpay.Actor.Interfaces;
﻿using Dotpay.Actor.Service.Interfaces;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using RabbitMQ.Client;

namespace Dotpay.Actor.Service.Implementations
{ 
    public class UserResetPasswordService : Orleans.Grain, IUserResetPasswordService
    {
        private const string UserMQExchange = Constants.UserMQName + Constants.ExechangeSuffix;
        private const string UserMQQueue = Constants.UserMQName + Constants.QueueSuffix;

        async Task IUserResetPasswordService.ForgetLoginPassword(long userId, Lang lang)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            var resetToken = await user.ForgetLoginPassword();
            var userInfo = await user.GetUserInfo();
            var forgetLoginPasswordMsg = new UserForgetLoginPasswordMessage(userInfo.Email, userInfo.LoginName,
                resetToken, DateTime.Now, lang);

            await MessageProducterManager.GetProducter().PublishMessage(forgetLoginPasswordMsg, UserMQExchange, "", true);
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

            await MessageProducterManager.GetProducter().PublishMessage(forgetPaymentPasswordMsg, UserMQExchange, "", true);
        }

        //此处独立出来，是为了以后在用户重置支付密码后，可以发邮件给用户
        Task IUserResetPasswordService.ResetPaymentPassword(long userId, string newPaymentPassword, string resetToken)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            return user.ResetLoginPassword(newPaymentPassword, resetToken);
        }

        public override Task OnActivateAsync()
        {
            MessageProducterManager.RegisterAndBindQueue(UserMQExchange, ExchangeType.Direct, UserMQQueue, durable: true);
            return base.OnActivateAsync();
        }
    }
}
