 
﻿using System;
﻿using System.Threading.Tasks;
﻿using Dotpay.Actor.Events;
﻿using Dotpay.Actor;
﻿using Dotpay.Common;
﻿using Orleans.EventSourcing;
﻿using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
    [StorageProvider(ProviderName = Constants.StorageProviderName)]
    public class SystemSetting : EventSourcingGrain<SystemSetting, ISystemSettingState>, ISystemSetting
    {
        Task<RippleToFinancialInstitutionSetting> ISystemSetting.GetRippleToFinancialInstitutionSetting()
        {
            return Task.FromResult(this.State.RippleToFinancialInstitutionSetting);
        }

        Task ISystemSetting.UpdateRippleToFinancialInstitutionSetting(RippleToFinancialInstitutionSetting setting, Guid updateBy)
        {
            return this.ApplyEvent(new RippleToFinancialInstitutionSettingUpdated(setting, updateBy));
        }

        public Task<RippleToDotpaySetting> GetRippleToDotpaySetting()
        {
            return Task.FromResult(this.State.RippleToDotpaySetting);
        }

        public Task UpdateRippleToDotpaySetting(RippleToDotpaySetting setting, Guid updateBy)
        {
            return this.ApplyEvent(new RippleToDotpaySettingUpdated(setting, updateBy));
        } 

        #region Event Handlers
        private void Handle(RippleToFinancialInstitutionSettingUpdated @event)
        {
            this.State.RippleToFinancialInstitutionSetting = @event.Setting;
            this.State.FISettingUpdateAt = @event.UTCTimestamp;
            this.State.FISettingUpdateBy = @event.UpdateBy;
            this.State.WriteStateAsync();
        }
        private void Handle(RippleToDotpaySettingUpdated @event)
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
        RippleToFinancialInstitutionSetting RippleToFinancialInstitutionSetting { get; set; }
        RippleToDotpaySetting RippleToDotpaySetting { get; set; }
        DateTime FISettingUpdateAt { get; set; }
        Guid FISettingUpdateBy { get; set; }
        DateTime DotpaySettingUpdateAt { get; set; }
        Guid DotpaySettingUpdateBy { get; set; }
    }


}
