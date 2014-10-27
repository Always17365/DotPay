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
    public class CNYDepositCommandTest
    {
        ICommandBus commandBus;
        Random numRandom;

        public CNYDepositCommandTest()
        {
            TestEnvironment.Init();
            numRandom = new Random();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }
         
    }
}
