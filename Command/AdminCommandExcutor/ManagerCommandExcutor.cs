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
                                        ICommandExecutor<CreateManagerCommand>,
                                        ICommandExecutor<AssignManagerRolesCommand>,
                                        ICommandExecutor<LockManagerCommand>,
                                        ICommandExecutor<UnlockManagerCommand>,
                                        ICommandExecutor<ResetLoginPasswordCommand>,
                                        ICommandExecutor<ResetTwofactorKeyCommand>
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

            if (!await manager.HasInitialized())
            {
                await manager.Initialize(cmd.LoginName, cmd.LoginPassword, twofactorKey, cmd.ManagerId);
                var roles = new List<ManagerType>
                {
                    ManagerType.SuperUser,
                    ManagerType.MaintenanceManager,
                    ManagerType.DepositManager,
                    ManagerType.TransferManager
                };
                await manager.AssignRoles(cmd.ManagerId, roles);
                cmd.CommandResult = ErrorCode.None;
            }
            else
            {
                cmd.CommandResult = ErrorCode.SuperManagerHasInitialized;
            }
        }

        public async Task ExecuteAsync(CreateManagerCommand cmd)
        {
            var managerService = GrainFactory.GetGrain<IManagerService>(0);
            cmd.CommandResult = await managerService.AddManager(cmd.LoginName, cmd.LoginPassword, cmd.CreateBy);
        }

        public async Task ExecuteAsync(AssignManagerRolesCommand cmd)
        {
            var managerService = GrainFactory.GetGrain<IManagerService>(0);
            cmd.CommandResult = await managerService.AssginManagerRoles(cmd.ManagerId, cmd.Roles, cmd.AssignBy);
        }

        public async Task ExecuteAsync(LockManagerCommand cmd)
        {
            var managerService = GrainFactory.GetGrain<IManagerService>(0);
            cmd.CommandResult = await managerService.Lock(cmd.ManagerId, cmd.Reason, cmd.LockBy);
        }

        public async Task ExecuteAsync(UnlockManagerCommand cmd)
        {
            var managerService = GrainFactory.GetGrain<IManagerService>(0);
            cmd.CommandResult = await managerService.Unlock(cmd.ManagerId, cmd.UnlockBy);
        }

        public async Task ExecuteAsync(ResetLoginPasswordCommand cmd)
        {
            var managerService = GrainFactory.GetGrain<IManagerService>(0);
            cmd.CommandResult = await managerService.ResetLoginPassword(cmd.ManagerId, cmd.NewLoginPassword, cmd.ResetBy);
        }

        public async Task ExecuteAsync(ResetTwofactorKeyCommand cmd)
        { 
            var managerService = GrainFactory.GetGrain<IManagerService>(0);
            cmd.CommandResult = await managerService.ResetTwofactorKey(cmd.ManagerId, cmd.ResetBy);
        }
    }
}
