using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor;
using Dotpay.Actor.Service;
using Dotpay.AdminCommand;
using Dotpay.Common;
using Orleans;

namespace Dotpay.AdminCommandExcutor
{
    public class ManagerCommandExcutor : ICommandExecutor<ManagerLoginCommand>,
                                        ICommandExecutor<CreateManagerCommand>
#if DEBUG
, ICommandExecutor<CreateSuperAdministratorCommand>
#endif
    {
        public async Task ExecuteAsync(ManagerLoginCommand cmd)
        {
            var manager = GrainFactory.GetGrain<IManager>(cmd.ManangerId);

            cmd.CommandResult = await manager.Login(cmd.LoginPassword, cmd.Ip); 
        }
        public async Task ExecuteAsync(CreateSuperAdministratorCommand cmd)
        {
            var manager = GrainFactory.GetGrain<IManager>(cmd.ManagerId);
            var twofactorKey = Utilities.GenerateOTPKey();
            await manager.Initialize(cmd.LoginName, cmd.LoginPassword, twofactorKey, cmd.ManagerId);
            var roles = new List<ManagerType>
            {
                ManagerType.SuperUser,ManagerType.MaintenanceManager,
                ManagerType.DepositManager,ManagerType.TransferManager
            };
            await manager.AssignRoles(cmd.ManagerId, roles);
        }

        public async Task ExecuteAsync(CreateManagerCommand cmd)
        {
            var managerService = GrainFactory.GetGrain<IManagerService>(0);
            cmd.CommandResult = await managerService.AddManager(cmd.LoginName, cmd.LoginPassword, cmd.CreateBy);
        }

    }
}
