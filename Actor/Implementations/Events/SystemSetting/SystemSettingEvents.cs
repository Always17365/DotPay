using System;
using Dotpay.Actor;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class RippleToFISettingUpdatedEvent : GrainEvent
    {
        public RippleToFISettingUpdatedEvent(RippleToFiSetting setting, Guid updateBy)
        {
            this.Setting = setting; 
            this.UpdateBy = updateBy;
        }

        public RippleToFiSetting Setting { get; private set; } 
        public Guid UpdateBy { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class RippleToDotpaySettingUpdatedEvent : GrainEvent
    {
        public RippleToDotpaySettingUpdatedEvent(RippleToDotpaySetting setting, Guid updateBy)
        {
            this.Setting = setting;
            this.UpdateBy = updateBy;
        }

        public RippleToDotpaySetting Setting { get; private set; }
        public Guid UpdateBy { get; private set; }
    } 
}
