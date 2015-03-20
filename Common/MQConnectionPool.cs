using FC.Framework;
using FC.Framework.Utilities;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotPay.Common
{
    public class MQConnectionPool : IDisposable
    {
        private bool _disposed = false;
        private object _locker = new object();

        public void Dispose()
        {
            if (!_disposed)
            {
                if (this._channeles != null && this._channeles.Count > 0)
                {
                    this._channeles.ForEach(m => { m.Value.Dispose(); });
                    this._channeles.Clear();
                }
            }
            GC.SuppressFinalize(this);
        }

        private ConnectionFactory _factory;
        private IConnection _connection;
        private Dictionary<string, IModel> _channeles = new Dictionary<string, IModel>();

        public MQConnectionPool(string mqConnectionString)
        {
            Check.Argument.IsNotEmpty(mqConnectionString, "mqConnectionString");

            _factory = new ConnectionFactory();
            _factory.Uri = mqConnectionString;
        }

        public IModel GetMQChannel(string mqname)
        {
            try
            {
                return _GetMQChannel(mqname);
            }
            catch (Exception ex)
            {
                var msg = "重新链接" + mqname + "的队列时出现错误,30秒后重新链接";
                Console.WriteLine(msg);
                Log.Error(msg, ex);
                Thread.Sleep(30 * 1000);
                return GetMQChannel(mqname);
            }
        }

        private IModel _GetMQChannel(string mqname)
        {
            IModel mqchannel;

            _channeles.TryGetValue(mqname, out mqchannel);

            if (!_channeles.TryGetValue(mqname, out mqchannel))
            {
                lock (_locker)
                {
                    if (_connection == null)
                    {
                        _connection = _factory.CreateConnection();
                    }
                    mqchannel = _connection.CreateModel();
                    _channeles[mqname] = mqchannel;
                }
            }

            return mqchannel;
        }
    }
}
