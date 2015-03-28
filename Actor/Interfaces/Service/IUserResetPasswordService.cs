using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Common.Enum;
﻿using Orleans;

namespace Dotpay.Actor.Service.Interfaces
{
     
    public interface IUserResetPasswordService : Orleans.IGrain
    {
        Task ForgetLoginPassword(Int64 userId,Lang lang);
        Task ResetLoginPassword(Int64 userId, string newLoginPassword, string resetToken);
        Task ForgetPaymentPassword(Int64 userId, Lang lang);
        Task ResetPaymentPassword(Int64 userId, string newPaymentPassword, string resetToken); 
    }
}
