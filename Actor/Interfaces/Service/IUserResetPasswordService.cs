using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Common.Enum;
﻿using Orleans;

namespace Dotpay.Actor.Service
{
     
    public interface IUserResetPasswordService : Orleans.IGrain
    {
        Task ForgetLoginPassword(long userId,Lang lang);
        Task ResetLoginPassword(long userId, string newLoginPassword, string resetToken);
        Task ForgetPaymentPassword(long userId, Lang lang);
        Task ResetPaymentPassword(long userId, string newPaymentPassword, string resetToken); 
    }
}
