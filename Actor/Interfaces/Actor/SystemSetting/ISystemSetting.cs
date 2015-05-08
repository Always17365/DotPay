using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor
{
    public interface ISystemSetting : IGrainWithIntegerKey
    {
        Task<RippleToFiSetting> GetRippleToFiSetting();
        Task UpdateRippleToFiSetting(RippleToFiSetting setting, Guid updateBy);
        Task<RippleToDotpaySetting> GetRippleToDotpaySetting();
        Task UpdateRippleToDotpaySetting(RippleToDotpaySetting setting, Guid updateBy);
    }

    [Immutable]
    [Serializable]
    public class RippleToFiSetting
    {
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal FixedFee { get; set; }
        public decimal FeeRate { get; set; }
        public decimal MinFee { get; set; }
        public decimal MaxFee { get; set; }
    }

    [Immutable]
    [Serializable]
    public class RippleToDotpaySetting
    {
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal FixedFee { get; set; }
        public decimal FeeRate { get; set; }
        public decimal MinFee { get; set; }
        public decimal MaxFee { get; set; }
    }
}