using System;
using System.Threading.Tasks;
using Dotpay.Common;
using Orleans;

namespace Dotpay.Actor.Service
{
    
    public interface ISystemSettingService : IGrainWithIntegerKey
    {
        Task<ErrorCode> UpdateRippleToFiSetting(RippleToFiSetting setting, Guid updateBy);
        Task<ErrorCode> UpdateRippleToDotpaySetting(RippleToDotpaySetting setting, Guid updateBy);
    }
}
