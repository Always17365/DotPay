using System;
using Dotpay.Actor.Interfaces;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class RippleToFinancialInstitutionSettingUpdated : GrainEvent
    {
        public RippleToFinancialInstitutionSettingUpdated(RippleToFinancialInstitutionSetting setting, Guid updateBy)
        {
            this.Setting = setting; 
            this.UpdateBy = updateBy;
        }

        public RippleToFinancialInstitutionSetting Setting { get; private set; } 
        public Guid UpdateBy { get; private set; }
    } 
}
