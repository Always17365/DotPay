using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor.Interfaces
{
    public interface ISystemSetting : IGrainWithIntegerKey
    {
        Task<RippleToFinancialInstitutionSetting> GetRippleToFinancialInstitutionSetting();
        Task UpdateRippleToFinancialInstitutionSetting(RippleToFinancialInstitutionSetting setting, Guid updateBy);
    }

    [Immutable]
    [Serializable]
    public class RippleToFinancialInstitutionSetting
    {
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal FixedFee { get; set; }
        public decimal FeeRate { get; set; }
        public decimal? MinFee { get; set; }
        public decimal? MaxFee { get; set; }
    }
}