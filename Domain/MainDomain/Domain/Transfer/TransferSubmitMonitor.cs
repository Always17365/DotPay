using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.MainDomain.Exceptions;
using DotPay.Tools.DistributedMessageSender;

namespace DotPay.MainDomain
{
    [Component]
    [AwaitCommitted]
    public class TransferSubmitMonitor : IEventHandler<InsideTransferTransactionCreated>
    {
        public void Handle(InsideTransferTransactionCreated @event)
        {
            var tx = @event.TransactionEntity;

            var msgID = @event.Currency.ToString() + "_inside_tx_" + tx.ID;
            var msg = new InsideTransactionCreateMessage(@event.TransactionEntity.ID, @event.FromUserID, @event.ToUserID, @event.Currency, @event.Amount, @event.PayWay, @event.Description);

            var exchangeName = Utilities.GenerateExchangeAndQueueNameOfInsideTransfer().Item1;

            var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(msg));
            var existKey = msgID;
            try
            {
                var obj = new Object();
                //在缓存中查找是否已发过该消息的证据，如果不存在，就发送消息出去
                if (!Cache.TryGet(existKey, out obj))
                {
                    Cache.Add(existKey, string.Empty);
                    MessageSender.Send(exchangeName, msgBytes, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error("用户提现内部转账id={0}的过程中出现错误:{1}", @event.TransactionEntity.ID, ex.Message);
            }
        }

        private class InsideTransactionCreateMessage : TransactionMessage
        {
            public InsideTransactionCreateMessage(int transferTxID, int fromUserID, int toUserID, CurrencyType currency, decimal amount, PayWay payway, string description)
            {
                this.TransferTxID = transferTxID;
                this.FromUserID = fromUserID;
                this.ToUserID = toUserID;
                this.Currency = currency;
                this.Amount = amount;
                this.PayWay = payway;
                this.Description = description;
                this.TransactionFlg = TransactionType.Inside;
            }
            public int FromUserID { get; private set; }
            public int ToUserID { get; private set; }
            public CurrencyType Currency { get; private set; }
            public decimal Amount { get; private set; }
            public PayWay PayWay { get; private set; }
            public string Description { get; private set; }
            public int TransferTxID { get; private set; }
        }

        private abstract class TransactionMessage
        {
            public TransactionType TransactionFlg { get; protected set; }
        }

        private enum TransactionType
        {
            Inside = 1,
            Outside = 2,
            Exchange = 3
        }
    }
}
