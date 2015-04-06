using System;
using Dotpay.Actor;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class RippleToFISettingUpdated : GrainEvent
    {
        public RippleToFISettingUpdated(RippleToFISetting setting, Guid updateBy)
        {
            this.Setting = setting; 
            this.UpdateBy = updateBy;
        }

        public RippleToFISetting Setting { get; private set; } 
        public Guid UpdateBy { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class RippleToDotpaySettingUpdated : GrainEvent
    {
        public RippleToDotpaySettingUpdated(RippleToDotpaySetting setting, Guid updateBy)
        {
            this.Setting = setting;
            this.UpdateBy = updateBy;
        }

        public RippleToDotpaySetting Setting { get; private set; }
        public Guid UpdateBy { get; private set; }
    } 
}
