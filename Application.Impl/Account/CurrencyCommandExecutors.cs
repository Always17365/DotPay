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
    public class CurrencyCommandExecutors : ICommandExecutor<CreateCurrency>,
                                            ICommandExecutor<EnableCurrency>,
                                            ICommandExecutor<DisableCurrency>,
                                            ICommandExecutor<ModifyCurrencyDepositFeeRate>,
                                            ICommandExecutor<ModifyCurrencyWithdrawFeeRate>,
                                            ICommandExecutor<ModifyCurrencyNeedConfirm>,
                                            ICommandExecutor<ModifyCurrencyWithdrawVerifyLine>,
                                            ICommandExecutor<ModifyCurrencyWithdrawDayLimit>,
                                            ICommandExecutor<ModifyCurrencyWithdrawOnceLimit>,
                                            ICommandExecutor<ModifyCurrencyWithdrawOnceMin>
    {
        public void Execute(CreateCurrency cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = new Currency(cmd.CurrencyID, cmd.CurrencyCode, cmd.CurrencyName, cmd.CreateBy);

            IoC.Resolve<IRepository>().Add(currency);
        }

        public void Execute(EnableCurrency cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<Currency>(cmd.CurrencyID);

            currency.Enable(cmd.ByUserID);
        }

        public void Execute(DisableCurrency cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<Currency>(cmd.CurrencyID);

            currency.Disable(cmd.ByUserID);
        }

        public void Execute(ModifyCurrencyDepositFeeRate cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<Currency>(cmd.CurrencyID);

            currency.ModifyDepositFeeRate(cmd.DepositFixedFee, cmd.DepositFeeRate, cmd.ByUserID);
        }

        public void Execute(ModifyCurrencyWithdrawFeeRate cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<Currency>(cmd.CurrencyID);

            currency.ModifyWithdrawFeeRate(cmd.WithdrawFixedFee, cmd.WithdrawFeeRate, cmd.ByUserID);
        }

        public void Execute(ModifyCurrencyNeedConfirm cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<Currency>(cmd.CurrencyID);

            currency.ModifyNeedConfirm(cmd.NeedConfirm, cmd.ByUserID);
        }

        public void Execute(ModifyCurrencyWithdrawVerifyLine cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<Currency>(cmd.CurrencyID);

            currency.ModifyWithdrawVerifyLine(cmd.VerifyLine, cmd.ByUserID);
        }

        public void Execute(ModifyCurrencyWithdrawDayLimit cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<Currency>(cmd.CurrencyID);

            currency.ModifyWithdrawDayLimit(cmd.DayLimit, cmd.ByUserID);
        }

        public void Execute(ModifyCurrencyWithdrawOnceLimit cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<Currency>(cmd.CurrencyID);

            currency.ModifyWithdrawOnceLimit(cmd.OnceLimit, cmd.ByUserID);
        }

        public void Execute(ModifyCurrencyWithdrawOnceMin cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<Currency>(cmd.CurrencyID);

            currency.ModifyWithdrawOnceMin(cmd.OnceMin, cmd.ByUserID);
        }
    }
}
