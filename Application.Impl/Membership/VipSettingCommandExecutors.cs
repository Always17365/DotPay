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
    public class VipSettingCommandExecutors : ICommandExecutor<SetVipLevelScoreLine>,
                                              ICommandExecutor<SetVipLevelDiscount>,
                                              ICommandExecutor<SetVipLevelVoteCount>
    { 
        public void Execute(SetVipLevelScoreLine cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var vipsetting = IoC.Resolve<IRepository>().FindById<VipSetting>(cmd.VipLevelId);

            vipsetting.ModifyScoreLine(cmd.ScoreLine, cmd.ByUserID);
        }

        public void Execute(SetVipLevelDiscount cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var vipsetting = IoC.Resolve<IRepository>().FindById<VipSetting>(cmd.VipLevelId);

            vipsetting.ModifyDiscount(cmd.Discount, cmd.ByUserID);
        }

        public void Execute(SetVipLevelVoteCount cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var vipsetting = IoC.Resolve<IRepository>().FindById<VipSetting>(cmd.VipLevelId);

            vipsetting.ModifyVoteCount(cmd.VoteCount, cmd.ByUserID);
        }

    }
}
