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
    public class DepositCommandExecutor : ICommandExecutor<ConfirmDepositCommand> 
    { 

        public async Task ExecuteAsync(ConfirmDepositCommand cmd)
        {
            var depositManager= GrainFactory.GetGrain<IDepositTransactionManager>(0);

            cmd.CommandResult = await depositManager.ConfirmDepositTransaction(cmd.DepositId,cmd.ConfrimBy,cmd.TransferNo,cmd.Amount);
        } 
    }
}
