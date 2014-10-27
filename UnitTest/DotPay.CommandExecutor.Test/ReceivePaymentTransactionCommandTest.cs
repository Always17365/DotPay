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
using FC.Framework.Utilities;

namespace DotPay.CommandExecutor.Test
{
    public class ReceivePaymentTransactionCommandTest
    {
        ICommandBus commandBus;

        public ReceivePaymentTransactionCommandTest()
        {
            TestEnvironment.Init();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }


        [Fact]
        public void TestCreateReceivePaymentTransactionAndConfirm()
        {
            var userID = new Random().Next(1, 10);
            var paymentAddress = Guid.NewGuid().Shrink();


        }
    }
}

