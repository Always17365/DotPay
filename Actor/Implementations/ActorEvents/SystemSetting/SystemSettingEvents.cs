using System;
using Dotpay.Actor.Interfaces;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    public class RippleToFinancialInstitutionSettingUpdated : GrainEvent
    {
        public RippleToFinancialInstitutionSettingUpdated(RippleToFinancialInstitutionSetting setting, Guid updateBy)
        {
            this.Setting = setting; 
            this.UpdateBy = updateBy;
        }

        public RippleToFinancialInstitutionSetting Setting { get; set; } 
        public Guid UpdateBy { get; set; }
    }


}
