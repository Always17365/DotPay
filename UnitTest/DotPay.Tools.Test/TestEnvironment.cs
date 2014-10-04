using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Log4net;
using FC.Framework.Repository;
using DotPay.Common;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace DotPay.Tools.Test
{
    public class TestEnvironment
    {
        static bool inited = false;

        public static void Init()
        {
            if (!inited)
            {
                FCFramework.Initialize().UseAutofac()
                                        .UseLog4net()
                                        .Start();
                inited = true;
            }
        }

    }
}
