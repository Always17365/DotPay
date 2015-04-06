using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Common;
using Orleans;

namespace Dotpay.Actor.Service
{
    
    public interface ISystemSettingService : Orleans.IGrainWithIntegerKey
    {
        Task<ErrorCode> UpdateRippleToFISetting(RippleToFISetting setting, Guid updateBy);
        Task<ErrorCode> UpdateRippleToDotpaySetting(RippleToDotpaySetting setting, Guid updateBy);
    }
}
