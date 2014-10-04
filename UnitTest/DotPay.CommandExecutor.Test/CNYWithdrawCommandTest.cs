using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.CouchbaseCache;
using FC.Framework.Log4net;
using FC.Framework.NHibernate;
using FC.Framework.Repository;
using DotPay.Command;
using DotPay.Common;
using DotPay.Persistence;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using DotPay.Domain.Repository;
using DotPay.Domain;
using FC.Framework.Utilities;
using System.Threading;

namespace DotPay.CommandExecutor.Test
{
    public class CNYWithdrawCommandTest
    {
        ICommandBus commandBus;
        Random numRandom;

        public CNYWithdrawCommandTest()
        {
            TestEnvironment.Init();
            numRandom = new Random();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }


        [Fact]
        public void TestSubmitCNYWithdraw_Common_Process_Flow()
        {
                      var userID = 0;
            var withdrawAmount = 0M;
            var bank = Bank.CCB;
            var bankAccount = "6222023400022443546";
            var bankAddress = "上海市XX路XX号";
            var openBankName = "上海市XX建设银行";

            for (int i = 0; i < 50; i++)
            {
                userID = i;

                var cnyAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(userID, CurrencyType.CNY);

                if (cnyAccount != null && cnyAccount.Balance > 0)
                {
                    var realName = "张三" + userID;
                    var idno = "210224195506031426" + userID;
                    var idType = IdNoType.IdentificationCard;

                    var realNameAuth = new UserRealNameAuth(userID, realName, idType, idno);

                    this.commandBus.Send(realNameAuth);
                    withdrawAmount = cnyAccount.Balance / 4;
                    break;
                }
            }

            var submitWithdrawCMD = new SubmitCNYWithdraw(null, userID, withdrawAmount, bank, bankAccount, "123456");

            Assert.DoesNotThrow(() => { this.commandBus.Send(submitWithdrawCMD); });

            for (int i = 0; i < 50; i++)
            {
                var cnyWithdraw = IoC.Resolve<IRepository>().FindById<CNYWithdraw>(i);

                if (cnyWithdraw != null && cnyWithdraw.State == WithdrawState.WaitVerify)
                {
                    var cnyAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(cnyWithdraw.UserID, CurrencyType.CNY);
                    var cnyLocked = cnyAccount.Locked;
                    var verifyWithdrawCMD = new CNYWithdrawVerify(cnyWithdraw.ID, "提现额度审核通过", userID);

                    Assert.DoesNotThrow(() => { this.commandBus.Send(verifyWithdrawCMD); });
                    cnyWithdraw = IoC.Resolve<IRepository>().FindById<CNYWithdraw>(i);

                    Assert.Equal(cnyWithdraw.State, WithdrawState.WaitSubmit);
                    Assert.True(cnyWithdraw.VerifyAt > 0);

                    var submitWithdrawToProcessCMD = new SubmitCNYWithdrawToProcess(cnyWithdraw.ID, userID);

                    Assert.DoesNotThrow(() => { this.commandBus.Send(submitWithdrawToProcessCMD); });
                    cnyWithdraw = IoC.Resolve<IRepository>().FindById<CNYWithdraw>(i);

                    Assert.Equal(cnyWithdraw.State, WithdrawState.Processing);
                    Assert.True(cnyWithdraw.SubmitAt > 0);

                    var transferAccount = 1;
                    var transferNO = "2014050131452186354664188416";
                    var cnyWithdrawMarkAsSuccessCMD = new CNYWithdrawMarkAsSuccess(cnyWithdraw.ID, transferAccount, transferNO, userID);

                    Assert.DoesNotThrow(() => { this.commandBus.Send(cnyWithdrawMarkAsSuccessCMD); });
                    cnyWithdraw = IoC.Resolve<IRepository>().FindById<CNYWithdraw>(i);

                    cnyAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(cnyWithdraw.UserID, CurrencyType.CNY);


                    Assert.Equal(cnyWithdraw.State, WithdrawState.Complete);
                    Assert.True(cnyWithdraw.DoneAt > 0);
                    Assert.Equal(cnyLocked, cnyAccount.Locked + cnyWithdraw.Amount);

                    break;
                }
            }

        }

        [Fact]
        public void TestSubmitCNYWithdraw_Fail_Process_Flow()
        {
            var userID = 0;
            var withdrawAmount = 0M;
            var bank = Bank.CCB;
            var bankAccount = "6222023400022443546";
            var bankAddress = "上海市XX路XX号";
            var openBankName = "上海市XX建设银行";

            for (int i = 0; i < 50; i++)
            {
                userID = i;

                var cnyAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(userID, CurrencyType.CNY);

                if (cnyAccount != null && cnyAccount.Balance > 0)
                {
                    var realName = "张三" + userID;
                    var idno = "210224195506031426" + userID;
                    var idType = IdNoType.IdentificationCard;

                    var realNameAuth = new UserRealNameAuth(userID, realName, idType, idno);

                    this.commandBus.Send(realNameAuth);
                    withdrawAmount = cnyAccount.Balance / 4;
                    break;
                }
            }

            var submitWithdrawCMD = new SubmitCNYWithdraw(null, userID, withdrawAmount, bank, bankAccount, "123456");

            Assert.DoesNotThrow(() => { this.commandBus.Send(submitWithdrawCMD); });

            for (int i = 0; i < 50; i++)
            {
                var cnyWithdraw = IoC.Resolve<IRepository>().FindById<CNYWithdraw>(i);

                if (cnyWithdraw != null && cnyWithdraw.State == WithdrawState.WaitVerify)
                {
                    var cnyAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(cnyWithdraw.UserID, CurrencyType.CNY);
                    var cnyLocked = cnyAccount.Locked;
                    var verifyWithdrawCMD = new CNYWithdrawVerify(cnyWithdraw.ID, "提现额度审核通过", userID);

                    Assert.DoesNotThrow(() => { this.commandBus.Send(verifyWithdrawCMD); });
                    cnyWithdraw = IoC.Resolve<IRepository>().FindById<CNYWithdraw>(i);

                    Assert.Equal(cnyWithdraw.State, WithdrawState.WaitSubmit);
                    Assert.True(cnyWithdraw.VerifyAt > 0);

                    var submitWithdrawToProcessCMD = new SubmitCNYWithdrawToProcess(cnyWithdraw.ID, userID);

                    Assert.DoesNotThrow(() => { this.commandBus.Send(submitWithdrawToProcessCMD); });
                    cnyWithdraw = IoC.Resolve<IRepository>().FindById<CNYWithdraw>(i);

                    Assert.Equal(cnyWithdraw.State, WithdrawState.Processing);
                    Assert.True(cnyWithdraw.SubmitAt > 0);

                    var memo = "打款错了，账号不对";
                    var cnyWithdrawMarkAsFailCMD = new CNYWithdrawMarkAsTransferFail(cnyWithdraw.ID, memo, userID);

                    Assert.DoesNotThrow(() => { this.commandBus.Send(cnyWithdrawMarkAsFailCMD); });
                    cnyWithdraw = IoC.Resolve<IRepository>().FindById<CNYWithdraw>(i);

                    Assert.Equal(cnyWithdraw.State, WithdrawState.Fail);

                    var oldReceiverAccountID = cnyWithdraw.ReceiverBankAccountID;
                    bank = Bank.ABC;
                    bankAccount = "6222023400022423478";
                    bankAddress = "上海市XX路XX号";
                    openBankName = "上海市XX农业银行";

                    var cnyWithdrawModifyReceiverBankAccoutCMD = new CNYWithdrawModifyReceiverBankAccount(cnyWithdraw.ID, bank, bankAccount, userID);

                    Assert.DoesNotThrow(() => { this.commandBus.Send(cnyWithdrawModifyReceiverBankAccoutCMD); });
                    cnyWithdraw = IoC.Resolve<IRepository>().FindById<CNYWithdraw>(i);

                    Assert.NotEqual(cnyWithdraw.ReceiverBankAccountID, oldReceiverAccountID);

                    var transferAccount = 1;
                    var transferNO = "2014050131452186354664188416";
                    var cnyWithdrawMarkAsSuccessCMD = new CNYWithdrawMarkAsSuccess(cnyWithdraw.ID, transferAccount, transferNO, userID);

                    Assert.DoesNotThrow(() => { this.commandBus.Send(cnyWithdrawMarkAsSuccessCMD); });
                    cnyWithdraw = IoC.Resolve<IRepository>().FindById<CNYWithdraw>(i);

                    cnyAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(cnyWithdraw.UserID, CurrencyType.CNY);


                    Assert.Equal(cnyWithdraw.State, WithdrawState.Complete);
                    Assert.True(cnyWithdraw.DoneAt > 0);
                    Assert.Equal(cnyLocked, cnyAccount.Locked + cnyWithdraw.Amount);
                    break;
                }
            }

        }

  
    }
}

