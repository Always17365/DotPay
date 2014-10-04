using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using DotPay.ViewModel;
using FC.Framework;
using System.Reflection;
using FC.Framework.Repository;

namespace DotPay.QueryService.Impl
{
    public static class QueryRegister
    {
        public static FCFramework RegisterQueryServices(this FCFramework framework, ConnectionString connectionString)
        {
            IoC.Register<ConnectionString>(connectionString, "queryConnectionString");

            var typesInCurrentAssembly = Assembly.GetAssembly(typeof(QueryRegister)).GetTypes();

            foreach (var type in typesInCurrentAssembly)
            {
                if (!type.IsClass || type.IsAbstract || type.IsGenericType || type.IsInterface) continue;

                if (type.BaseType == typeof(AbstractQuery))
                {
                    IoC.Register(type.GetInterfaces().FirstOrDefault(), type, LifeStyle.Singleton);
                }

            };

            return framework;
        }
    }
}
