using System;
using System.Linq;
using System.Reflection;
using DFramework;
using MongoDB.Driver;

namespace Dotpay.FrontQueryServiceImpl
{
    internal class MongoManager
    {
        private static IMongoDatabase _database;
        private static bool _initialized = false;
        public static void Initialize(string connectionString, string databaseName)
        {
            if (_initialized) return;
            MongoClient client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            _initialized = true;
        }

        public static IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            if (!_initialized) throw new Exception("mongoManager uninitialize");

            return _database.GetCollection<T>(collectionName);
        }
    }

    public static class DFrameworkExtension
    {
        public static DEnvironment RegisterQueryService(this DEnvironment enviroment, string connectionString, string databaseName)
        {
            MongoManager.Initialize(connectionString, databaseName);

            var typesInCurrentAssembly = Assembly.GetAssembly(typeof(DFrameworkExtension)).GetTypes();

            foreach (var type in typesInCurrentAssembly)
            {
                if (!type.IsClass || type.IsAbstract || type.IsGenericType || type.IsInterface) continue;

                if (type.CustomAttributes.Any(ca => ca.AttributeType == typeof(QueryServiceAttribute)))
                {
                    IoC.Register(type.GetInterfaces().FirstOrDefault(), type, LifeStyle.Singleton);
                }
            };

            return enviroment;
        }
    }

    public class QueryServiceAttribute : Attribute
    {

    }
}
