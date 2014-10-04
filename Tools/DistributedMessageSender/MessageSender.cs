using FC.Framework;
using DotPay.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotPay.Tools.DistributedMessageSender
{
    /// <summary>
    /// thread safe message sender
    /// </summary>
    public class MessageSender
    {
        private static ThreadLocal<MQConnectionPool> _mqpool = new ThreadLocal<MQConnectionPool>(() => new MQConnectionPool(Config.MQConnectionString));

        public static void Send(string exchangeName, byte[] messageBody, bool durable = false, string routeKey = "")
        {  
            var _channel = _mqpool.Value.GetMQChannel("DotPay.Tools.DistributedMessageSender");

            var build = new BytesMessageBuilder(_channel);
            build.WriteBytes(messageBody);

            //是否持久化消息
            if (durable) ((IBasicProperties)build.GetContentHeader()).DeliveryMode = 2;

            try
            {
                _channel.BasicPublish(exchangeName, routeKey, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ex)
            {
                Log.Warn("messagesender发送消息时，发现消息队列服务器已关闭",ex);

                try
                {
                    _channel = _mqpool.Value.GetMQChannel("DotPay.Tools.DistributedMessageSender");
                    _channel.BasicPublish(exchangeName, string.Empty, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
                }
                catch (Exception eex)
                {
                    Log.Error("分布式消息发送器链接消息队列出错", eex);
                }
            }
            catch (System.IO.EndOfStreamException ex)
            {
                Log.Warn("messagesender发送消息时，发现消息队列服务器已关闭",ex);

                try
                {
                    _channel = _mqpool.Value.GetMQChannel("DotPay.Tools.DistributedMessageSender");
                    _channel.BasicPublish(exchangeName, string.Empty, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
                }
                catch (Exception eex)
                {
                    Log.Error("分布式消息发送器链接消息队列出错", eex);
                }
            }
        }

    }
}
