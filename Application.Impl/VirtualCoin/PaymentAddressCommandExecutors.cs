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
using DotPay.Tools.DistributedMessageSender;

namespace DotPay.Command.Executor.Trade
{
    public class PaymentAddressCommandExecutors : ICommandExecutor<CreatePaymentAddress>,
                                                  ICommandExecutor<GeneratePaymentAddress>
    {
        public void Execute(CreatePaymentAddress cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var accountRepos = IoC.Resolve<IAccountRepository>();
            var account = accountRepos.FindByUserIDAndCurrency(cmd.UserID, cmd.Currency);

            if (account == null)
            {
                account = AccountFactory.CreateAccount(cmd.UserID, cmd.Currency);
                accountRepos.Add(account);
            }

            var paymentAddress = PaymentAddressFactory.CreatePaymentAddress(cmd.UserID, account.ID, cmd.PaymentAddress, cmd.Currency);

            IoC.Resolve<IRepository>().Add(paymentAddress);
        }

        public void Execute(GeneratePaymentAddress cmd)
        {
            var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(cmd));

            var exchangePair = Utilities.GenerateExchangeAndQueueNameOfGenerateNewAddress(cmd.Currency);

            MessageSender.Send(exchangePair.Item1, msgBytes, true);
        }
    }
}