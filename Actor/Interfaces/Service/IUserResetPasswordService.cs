using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Common;
using Dotpay.Common.Enum;
﻿using Orleans;

namespace Dotpay.Actor.Service
{

    public interface IUserResetPasswordService : Orleans.IGrainWithIntegerKey
    {
        Task<ErrorCode> ForgetLoginPassword(Guid userId, Lang lang);
        Task<ErrorCode> ResetLoginPassword(Guid userId, string newLoginPassword, string resetToken);
        Task<ErrorCode> ForgetPaymentPassword(Guid userId, Lang lang);
        Task<ErrorCode> ResetPaymentPassword(Guid userId, string newPaymentPassword, string resetToken);
    }
}
