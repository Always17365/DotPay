using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor;
using Dotpay.AdminCommand;
using Orleans;

namespace Dotpay.AdminCommandExcutor
{
    public class SystemSettingCommandExcutor : ICommandExecutor<UpdateToFISettingCommand>,
                                               ICommandExecutor<UpdateToDotpaySettingCommand>
    {
        public Task ExecuteAsync(UpdateToFISettingCommand cmd)
        {
            var systemSetting = GrainFactory.GetGrain<ISystemSetting>(0);
            var setting = new RippleToFinancialInstitutionSetting()
            {
                MinAmount = cmd.MinAmount,
                MaxAmount = cmd.MaxAmount,
                FixedFee = cmd.FixedFee,
                FeeRate = cmd.FeeRate,
                MinFee = cmd.MinFee,
                MaxFee = cmd.MaxFee
            };
            return systemSetting.UpdateRippleToFinancialInstitutionSetting(setting, cmd.UpdateBy);
        }

        public Task ExecuteAsync(UpdateToDotpaySettingCommand cmd)
        {
            var systemSetting = GrainFactory.GetGrain<ISystemSetting>(0);
            var setting = new RippleToDotpaySetting()
            {
                MinAmount = cmd.MinAmount,
                MaxAmount = cmd.MaxAmount,
                FixedFee = cmd.FixedFee,
                FeeRate = cmd.FeeRate,
                MinFee = cmd.MinFee,
                MaxFee = cmd.MaxFee
            };
            return systemSetting.UpdateRippleToDotpaySetting(setting, cmd.UpdateBy);
        }
    }
}
