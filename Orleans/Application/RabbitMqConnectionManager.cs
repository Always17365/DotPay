using DFramework;
using RabbitMQ.Client;

namespace Dotpay.Application
{
    internal class RabbitMqConnectionManager
    {
        private static IConnection connection;
        private static object _locker=new object();
        
        public static IConnection GetConnection()
        {
            if (connection == null)
            {
                lock (_locker)
                {
                    if (connection == null)
                    {
                        var factory = IoC.Resolve<IConnectionFactory>();
                        connection = factory.CreateConnection();
                    }
                }
            }
            return connection; 
        } 
    }
}
