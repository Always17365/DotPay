 
﻿using System;
﻿using System.Threading.Tasks;
﻿using Dotpay.Actor.Events;
﻿using Dotpay.Common;
﻿using Orleans.EventSourcing;
﻿using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
    [StorageProvider(ProviderName = Constants.StorageProviderName)]
    public class SystemSetting : EventSourcingGrain<SystemSetting, ISystemSettingState>, ISystemSetting
    {
        Task<RippleToFiSetting> ISystemSetting.GetRippleToFiSetting()
        {
            return Task.FromResult(this.State.RippleToFISetting);
        }

        Task ISystemSetting.UpdateRippleToFiSetting(RippleToFiSetting setting, Guid updateBy)
        {
            return this.ApplyEvent(new RippleToFISettingUpdatedEvent(setting, updateBy));
        }

        public Task<RippleToDotpaySetting> GetRippleToDotpaySetting()
        {
            return Task.FromResult(this.State.RippleToDotpaySetting);
        }

        public Task UpdateRippleToDotpaySetting(RippleToDotpaySetting setting, Guid updateBy)
        {
            return this.ApplyEvent(new RippleToDotpaySettingUpdatedEvent(setting, updateBy));
        } 

        #region Event Handlers
        private void Handle(RippleToFISettingUpdatedEvent @event)
        {
            this.State.RippleToFISetting = @event.Setting;
            this.State.FISettingUpdateAt = @event.UTCTimestamp;
            this.State.FISettingUpdateBy = @event.UpdateBy;
            this.State.WriteStateAsync();
        }
        private void Handle(RippleToDotpaySettingUpdatedEvent @event)
        {
            this.State.RippleToDotpaySetting = @event.Setting;
            this.State.DotpaySettingUpdateAt = @event.UTCTimestamp;
            this.State.DotpaySettingUpdateBy = @event.UpdateBy;
            this.State.WriteStateAsync();
        }
        #endregion
    }

    public interface ISystemSettingState : IEventSourcingState
    {
        RippleToFiSetting RippleToFISetting { get; set; }
        RippleToDotpaySetting RippleToDotpaySetting { get; set; }
        DateTime FISettingUpdateAt { get; set; }
        Guid FISettingUpdateBy { get; set; }
        DateTime DotpaySettingUpdateAt { get; set; }
        Guid DotpaySettingUpdateBy { get; set; }
    }


}
