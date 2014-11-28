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
                                         IEventHandler<RippleInboundTxToThirdPartyPaymentCompelted>
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
                var obj = new Object();
                //在缓存中查找是否已发过该消息的证据，如果不存在，就发送消息出去 
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

            var msg = new RippleInboundToThirdPartyPaymentTxMessage(tx.Destination, @event.Amount, @event.PayWay,@event.RippleTxID);

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
    }
}
