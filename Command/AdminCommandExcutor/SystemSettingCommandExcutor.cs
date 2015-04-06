using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor;
using Dotpay.Actor.Service;
using Dotpay.AdminCommand;
using Orleans;

namespace Dotpay.AdminCommandExcutor
{
    public class SystemSettingCommandExcutor : ICommandExecutor<UpdateToFISettingCommand>,
                                               ICommandExecutor<UpdateToDotpaySettingCommand>
    {
        public async Task ExecuteAsync(UpdateToFISettingCommand cmd)
        {
            var systemSettingService = GrainFactory.GetGrain<ISystemSettingService>(0);
            var setting = new RippleToFISetting()
            {
                MinAmount = cmd.MinAmount,
                MaxAmount = cmd.MaxAmount,
                FixedFee = cmd.FixedFee,
                FeeRate = cmd.FeeRate,
                MinFee = cmd.MinFee,
                MaxFee = cmd.MaxFee
            };
            cmd.CommandResult = await systemSettingService.UpdateRippleToFISetting(setting, cmd.UpdateBy);
        }

        public async Task ExecuteAsync(UpdateToDotpaySettingCommand cmd)
        {
            var systemSettingService = GrainFactory.GetGrain<ISystemSettingService>(0);
            var setting = new RippleToDotpaySetting()
            {
                MinAmount = cmd.MinAmount,
                MaxAmount = cmd.MaxAmount,
                FixedFee = cmd.FixedFee,
                FeeRate = cmd.FeeRate,
                MinFee = cmd.MinFee,
                MaxFee = cmd.MaxFee
            };
            cmd.CommandResult = await systemSettingService.UpdateRippleToDotpaySetting(setting, cmd.UpdateBy);
        }
    }
}
