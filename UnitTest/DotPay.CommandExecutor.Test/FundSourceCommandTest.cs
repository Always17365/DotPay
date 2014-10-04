using FC.Framework;
using FC.Framework.Repository;
using DotPay.Command;
using DotPay.Common;
using DotPay.Domain;
using DotPay.QueryService;
using DotPay.QueryService.Impl;
using System.Linq;
using System;
using Xunit;

namespace DotPay.CommandExecutor.Test
{
    public class FundSourceCommandTest
    {
        ICommandBus commandBus;
        Random numRandom;

        public FundSourceCommandTest()
        {
            TestEnvironment.Init();
            numRandom = new Random();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }

      
     
    }
}
