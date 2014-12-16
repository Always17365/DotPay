using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.RippleDomain.Events;
using DotPay.RippleDomain.Repository;
using DotPay.Common;
using FC.Framework.Utilities;
using System.Web;
using FC.Framework.Repository;
using DotPay.Tools.DistributedMessageSender;


namespace DotPay.RippleDomain
{
    [Component]
    [AwaitCommitted]
    public class RippleTransferMonitor : IEventHandler<RippleInboundTxCreated>,
                                         IEventHandler<RippleInboundTxToThirdPartyPaymentCompelted>,
                                         IEventHandler<RippleOutboundTransferTxCreated>,
                                         IEventHandler<RippleOutboundTransferSigned>,
                                         IEventHandler<RippleOutboundTransferSubmitSuccess>,
                                         IEventHandler<RippleOutboundTransferSubmitFail>
    {
        public void Handle(RippleInboundTxCreated @event)
        {
            //当接收到ripple转账时，发送消息给dotpay处理充值
            var userID = @event.DestinationTag;
            var amount = @event.Amount;

            var msg = new RippleInboundTxMessage(userID, @event.RippleTxID, amount);

            var exchangeName = Utilities.GenerateExchangeAndQueueNameOfInboundTransfer().Item1;

            var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(msg));

            try
            {
                MessageSender.Send(exchangeName, msgBytes, true);
            }
            catch (Exception ex)
            {
                Log.Error("发送RippleInbound交易消息出现异常,txid=" + @event.RippleTxID, ex);
            }
        }

        public void Handle(RippleInboundTxToThirdPartyPaymentCompelted @event)
        {
            //当接收到ripple转账时，发送消息给dotpay处理，创建第三方支付转账数据，之后等待人工处理 
            var tx = IoC.Resolve<IInboundToThirdPartyPaymentTxRepository>().FindByTxIdAndPayway(@event.RippleTxID, @event.PayWay);

            var msg = new RippleInboundToThirdPartyPaymentTxMessage(tx.Destination, @event.Amount, @event.PayWay, @event.RippleTxID);

            var exchangeName = Utilities.GenerateExchangeAndQueueNameOfInboundTransfer().Item1;

            var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(msg));

            try
            {
                var obj = new Object();
                //在缓存中查找是否已发过该消息的证据，如果不存在，就发送消息出去 
                MessageSender.Send(exchangeName, msgBytes, true);
            }
            catch (Exception ex)
            {
                Log.Error("发送RippleInbound交易消息出现异常,txid=" + @event.RippleTxID, ex);
            }
        }

        public void Handle(RippleOutboundTransferTxCreated @event)
        {
            //当接收到ripple对外转账请求创建成功消息后，发送消息给RippleMonitor，等待RippleMonitor签名并提交该交易到Ripple网络中 

            var msg = new RippleOutboundTxMessage(@event.Destination, @event.DestinationTag, @event.TargetAmount, @event.TargetCurrency, @event.SourceSendMaxAmount, @event.RipplePaths);

            var exchangeName = Utilities.GenerateExchangeAndQueueNameOfOutboundTransferForSign().Item1;

            var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(msg));

            try
            {
                MessageSender.Send(exchangeName, msgBytes, true);
            }
            catch (Exception ex)
            {
                Log.Error("发送RippleOutbound交易消息出现异常,msg=" + IoC.Resolve<IJsonSerializer>().Serialize(msg), ex);
            }
        }
        public void Handle(RippleOutboundTransferSigned @event)
        {
            //当接收到ripple对外转账请求创建成功消息后，发送消息给RippleMonitor，等待RippleMonitor签名并提交该交易到Ripple网络中
            var msg = new RippleOutboundSignedMessage(@event.Txhash, @event.Txblob);

            var exchangeName = Utilities.GenerateExchangeAndQueueNameOfOutboundTransferForSubmit().Item1;

            var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(msg));

            try
            {
                MessageSender.Send(exchangeName, msgBytes, true);
            }
            catch (Exception ex)
            {
                Log.Error("发送RippleOutbound交易消息出现异常,msg=" + IoC.Resolve<IJsonSerializer>().Serialize(msg), ex);
            }
        }


        public void Handle(RippleOutboundTransferSubmitSuccess @event)
        {
            //当接收到ripple对外转账处理成功消息后，发送消息给TransferMonitor，等待RippleMonitor签名并提交该交易到Ripple网络中
            var msg = new RippleOutboundSubmitedMessage(true, @event.TxId);

            var exchangeName = Utilities.GenerateExchangeAndQueueNameOfOutboundTransferProcessComplete().Item1;

            var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(msg));

            try
            {
                MessageSender.Send(exchangeName, msgBytes, true);
            }
            catch (Exception ex)
            {
                Log.Error("发送RippleOutbound 处理成功消息时出现异常,msg=" + IoC.Resolve<IJsonSerializer>().Serialize(msg), ex);
            }
        }

        public void Handle(RippleOutboundTransferSubmitFail @event)
        {
            //当接收到ripple对外转账请求创建成功消息后，发送消息给RippleMonitor，等待RippleMonitor签名并提交该交易到Ripple网络中
            var msg = new RippleOutboundSubmitedMessage(false, @event.TxId, @event.Reason);

            var exchangeName = Utilities.GenerateExchangeAndQueueNameOfOutboundTransferProcessComplete().Item1;

            var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(msg));

            try
            {
                MessageSender.Send(exchangeName, msgBytes, true);
            }
            catch (Exception ex)
            {
                Log.Error("发送RippleOutbound 处理成功消息时出现异常,msg=" + IoC.Resolve<IJsonSerializer>().Serialize(msg), ex);
            }
        }

        #region 实体类

        [Serializable]
        private class RippleInboundTxMessage
        {
            public RippleInboundTxMessage(int toUserID, string txid, decimal amount)
            {
                this.ToUserID = ToUserID;
                this.TxId = txid;
                this.Amount = amount;
                this.PayWay = PayWay.Ripple;
            }
            public int ToUserID { get; private set; }
            public string TxId { get; private set; }
            public decimal Amount { get; private set; }
            public PayWay PayWay { get; private set; }
        }

        [Serializable]
        private class RippleInboundToThirdPartyPaymentTxMessage
        {
            public RippleInboundToThirdPartyPaymentTxMessage(string account, decimal amount, PayWay payway, string txid)
            {
                this.Account = account;
                this.TxId = txid;
                this.Amount = amount;
                this.PayWay = payway;
                this.SourcePayWay = PayWay.Ripple;
            }
            public string Account { get; private set; }
            public string TxId { get; private set; }
            public decimal Amount { get; private set; }
            public PayWay PayWay { get; private set; }
            public PayWay SourcePayWay { get; private set; }
        }

        [Serializable]
        private class RippleOutboundTxMessage
        {
            public RippleOutboundTxMessage(string destination, int destinationtag, decimal amount, string currency, decimal sendMax, List<List<object>> paths)
            {
                this.Destination = destination;
                this.DestinationTag = destinationtag;
                this.Amount = amount;
                this.Currency = currency;
                this.SendMax = sendMax;
                this.Paths = paths;
            }
            public string Destination { get; private set; }
            public int DestinationTag { get; private set; }
            public decimal Amount { get; private set; }
            public decimal SendMax { get; private set; }
            public string Currency { get; private set; }
            public List<List<object>> Paths { get; private set; }
        }

        [Serializable]
        private class RippleOutboundSignedMessage
        {
            public RippleOutboundSignedMessage(string txid, string txblob)
            {
                this.TxId = txid;
                this.TxBlob = txblob;
            }
            public string TxId { get; private set; }
            public string TxBlob { get; private set; }
        }

        [Serializable]
        private class RippleOutboundSubmitedMessage
        {
            public RippleOutboundSubmitedMessage(bool result, string txid, string reason = "")
            {
                this.Result = result;
                this.TxId = txid;
                this.Reason = reason;
            }
            public bool Result { get; private set; }
            public string TxId { get; private set; }
            public string Reason { get; private set; }
        }
        #endregion
    }
}
