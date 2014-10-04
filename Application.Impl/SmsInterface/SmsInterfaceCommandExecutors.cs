using FC.Framework;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.Domain;
using DotPay.Domain.Exceptions;
using DotPay.Domain.Repository;
using DotPay.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command.Executor
{
    public class SmsInterfaceExecutors : ICommandExecutor<CreateSmsInterface>,
                                         ICommandExecutor<ModifySmsInterface>
    {
        public void Execute(CreateSmsInterface cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var smsInterface = new SmsInterface(cmd.SmsType, cmd.Account, cmd.Password, cmd.CreateBy);

            IoC.Resolve<IRepository>().Add(smsInterface);
        }

        public void Execute(ModifySmsInterface cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var smsInterface = IoC.Resolve<IRepository>().FindById<SmsInterface>(cmd.SmsInterfaceID);

            smsInterface.Modify(cmd.SmsType, cmd.Account, cmd.Password, cmd.ByUserID);
        }
    }
}
