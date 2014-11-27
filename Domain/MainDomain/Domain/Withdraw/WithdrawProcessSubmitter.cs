using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.MainDomain.Events;
using FC.Framework.Repository;
using DotPay.MainDomain.Repository;
using DotPay.Tools.DistributedMessageSender;

/*
namespace DotPay.MainDomain
{
    /// <summary>
    /// 
    /// <remarks>在提现事务提交之后，提现处理指令才可真正被发出去，并且需要做防重复发送</remarks>
    /// </summary>
    [AwaitCommitted]
    [Component]
    public class WithdrawProcessSubmitter : IEventHandler<VirtualCoinWithdrawVerified>,
                                            IEventHandler<VirtualCoinWithdrawSkipVerify>
    {
        public void Handle(VirtualCoinWithdrawVerified @event)
        {
            var withdraw = IoC.Resolve<IWithdrawRepository>().FindByIdAndCurrency(@event.WithdrawID, @event.Currency) as VirtualCoinWithdraw;

            var msgID = @event.Currency.ToString() + "__withdraw_" + @event.WithdrawUniqueID;
            var msg = new VirtualCoinWithdrawProcessMessage(@event.Currency, @event.WithdrawUniqueID, msgID, withdraw.Amount, withdraw.ReceiveAddress);

            var exchangeName = Utilities.GenerateVirtualCoinSendPaymentExchangeAndQueueName(@event.Currency).Item1;

            var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(msg));
            var existKey = msgID + "_" + @event.Amount;
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
                Log.Error("用户提交提现后(提现ID={0})，发送自动提现指令过程中出现错误:{1}", @event.WithdrawUniqueID, ex.Message);
            }
        }

         public void Handle(VirtualCoinWithdrawSkipVerify @event)
        {
            var withdraw = @event.WithdrawEntity;

            var msgID = @event.Currency.ToString() + "__withdraw_" + @event.WithdrawUniqueID;
            var msg = new VirtualCoinWithdrawProcessMessage(@event.Currency, @event.WithdrawUniqueID, msgID, withdraw.Amount, withdraw.ReceiveAddress);

            var exchangeName = Utilities.GenerateVirtualCoinSendPaymentExchangeAndQueueName(@event.Currency).Item1;

            var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(msg));

            var existKey = msgID + "_" + @event.WithdrawEntity.Amount;
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
                Log.Error("用户提交提现后(提现ID={0})，发送自动提现指令过程中出现错误:{1}", @event.WithdrawUniqueID, ex.Message);
            }

        }
        private class VirtualCoinWithdrawProcessMessage
        {
            public VirtualCoinWithdrawProcessMessage(CurrencyType currency, string withdrawUniqueID, string msgID, decimal amount, string address)
            {
                this.Currency = currency;
                this.WithdrawUniqueID = withdrawUniqueID;
                this.MsgID = msgID;
                this.Amount = amount;
                this.Address = address;
            }

            public CurrencyType Currency { get; private set; }
            public string WithdrawUniqueID { get; private set; }
            public string MsgID { get; private set; }
            public decimal Amount { get; private set; }
            public string Address { get; private set; }
        }
         
    }
}
*/
