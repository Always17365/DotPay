using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Actor.Service;
﻿using Dotpay.Common;
﻿using Orleans;

namespace Dotpay.Actor.Service.Implementations
{
    public class SystemSettingService : Orleans.Grain, ISystemSettingService
    {
        async Task<ErrorCode> ISystemSettingService.UpdateRippleToFISetting(
            RippleToFISetting setting, Guid updateBy)
        {
            if (await CheckManagerPermission(updateBy))
            {
                var systemSetting = GrainFactory.GetGrain<ISystemSetting>(0);
                await systemSetting.UpdateRippleToFISetting(setting, updateBy);
                return ErrorCode.None;
            }
            else return ErrorCode.HasNoPermission;
        }

        async Task<ErrorCode> ISystemSettingService.UpdateRippleToDotpaySetting(RippleToDotpaySetting setting, Guid updateBy)
        {
            if (await CheckManagerPermission(updateBy))
            {
                var systemSetting = GrainFactory.GetGrain<ISystemSetting>(0);
                await systemSetting.UpdateRippleToDotpaySetting(setting, updateBy);
                return ErrorCode.None;
            }
            else return ErrorCode.HasNoPermission;
        }
        private Task<bool> CheckManagerPermission(Guid operatorId)
        {
            var @operator = GrainFactory.GetGrain<IManager>(operatorId);

            return @operator.HasRole(ManagerType.MaintenanceManager);
        }
    }
}
