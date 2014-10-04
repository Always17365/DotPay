using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using System.Reflection;
using FC.Framework;
using FC.Framework.Utilities;
using FC.Framework.Repository;

namespace DotPay.QueryService.Impl
{
    public abstract class AbstractQuery
    {
        private static IDbContext _context;
        private static object _obj_locker = new object();//lock
        private static string _connectionString;
        private static IDbProvider _provider;
        private static string _connectionStringName = "queryConnectionString";

        protected  IDbContext Context
        {
            get
            {
                if (_context == null)
                {
                    lock (_obj_locker)
                    {
                        if (_context == null)
                        {
                            _context = new DbContext().ConnectionString(_connectionString, _provider);
                            _context.IgnoreIfAutoMapFails(true);
                        }
                    }
                }
                return _context;
            }
        }

        public AbstractQuery()
            : this(IoC.Resolve<ConnectionString>(_connectionStringName).Value, IoC.Resolve<ConnectionString>(_connectionStringName).ProviderName)
        {

        }

        public AbstractQuery(string connectionString, string providerName)
        {
            Check.Argument.IsNotEmpty(connectionString, "connectionString");
            Check.Argument.IsNotEmpty(providerName, "providerName");
            _connectionString = connectionString;
            _provider = DBProviderFactory.CreateProvider(providerName);
        }

        public class DBProviderFactory
        {
            public static IDbProvider CreateProvider(string providerName)
            {
                IDbProvider provider = new SqlServerProvider(); //如果连接字符中为设置ProviderName,则默认指定使用SQL数据库

                var assembly = Assembly.GetAssembly(typeof(IDbProvider));
                var types = assembly.GetTypes();

                types.ForEach(t =>
                {
                    if (t.GetInterface("IDbProvider", true) != null
                        && t.GetProperty("ProviderName") != null
                        && t.GetProperty("ProviderName").GetValue(null, null).ToString() == providerName)
                    {
                        provider = assembly.CreateInstance(t.FullName) as IDbProvider;
                    }
                });

                return provider;
            }
        }
    }
}
