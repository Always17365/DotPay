using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;

namespace Dotpay.AdminCommand
{
    public class UpdateToFISettingCommand : Command
    {
        public UpdateToFISettingCommand(decimal minAmount, decimal maxAmount, decimal fixedFee, decimal feeRate, decimal minFee, decimal maxFee, Guid updateBy)
        {
            this.MinAmount = minAmount;
            this.MaxAmount = maxAmount;
            this.FixedFee = fixedFee;
            this.FeeRate = feeRate;
            this.MinFee = minFee;
            this.MaxFee = maxFee;
            this.UpdateBy = updateBy;
        }

        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal FixedFee { get; set; }
        public decimal FeeRate { get; set; }
        public decimal MinFee { get; set; }
        public decimal MaxFee { get; set; }
        public Guid UpdateBy { get; set; }
    }

    public class UpdateToDotpaySettingCommand : Command
    {
        public UpdateToDotpaySettingCommand(decimal minAmount, decimal maxAmount, decimal fixedFee, decimal feeRate, decimal minFee, decimal maxFee, Guid updateBy)
        {
            this.MinAmount = minAmount;
            this.MaxAmount = maxAmount;
            this.FixedFee = fixedFee;
            this.FeeRate = feeRate;
            this.MinFee = minFee;
            this.MaxFee = maxFee;
            this.UpdateBy = updateBy;
        }

        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal FixedFee { get; set; }
        public decimal FeeRate { get; set; }
        public decimal MinFee { get; set; }
        public decimal MaxFee { get; set; }
        public Guid UpdateBy { get; set; }

    }
}
