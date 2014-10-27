using FC.Framework;
using FC.Framework.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Persistence
{
    public static class RepositoryRegisterExtension
    {
        public static FCFramework RegisterAllRepository(this FCFramework framework)
        {
            var interfaceName = typeof(IRepository).FullName;
            var typesInCurrentAssembly = Assembly.GetAssembly(typeof(RepositoryRegisterExtension)).GetTypes();

            typesInCurrentAssembly.ForEach(type =>
            {
                var interfaces = type.GetInterfaces();
                var typeIsInheritInterface = interfaces.Count(q => q.FullName == interfaceName) > 0;
                if (typeIsInheritInterface)
                {
                    var realInterface = interfaces.SingleOrDefault(q => q.FullName != interfaceName);

                    if (realInterface != null)
                        IoC.Register(realInterface, type);
                }
            });

            return framework;

        }
    }
   
}
