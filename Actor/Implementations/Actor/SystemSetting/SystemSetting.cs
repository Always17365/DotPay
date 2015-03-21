 
﻿using System;
﻿using System.Collections.Generic;
﻿using System.Threading.Tasks;
﻿using Dotpay.Actor.Events;
﻿using Dotpay.Actor.Interfaces;
﻿using Orleans.EventSourcing;
﻿using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
    [StorageProvider(ProviderName = "CouchbaseStore")]
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

        #region Event Handlers
        private void Handle(RippleToFinancialInstitutionSettingUpdated @event)
        {
            this.State.RippleToFinancialInstitutionSetting = @event.Setting;
            this.State.RippleToFinancialInstitutionSettingUpdateAt = @event.UTCTimestamp;
            this.State.RippleToFinancialInstitutionSettingUpdateBy = @event.UpdateBy;
        }
        #endregion


    }

    public interface ISystemSettingState : IEventSourcingState
    {
        RippleToFinancialInstitutionSetting RippleToFinancialInstitutionSetting { get; set; }
        DateTime RippleToFinancialInstitutionSettingUpdateAt { get; set; }
        Guid RippleToFinancialInstitutionSettingUpdateBy { get; set; }
    }


}
