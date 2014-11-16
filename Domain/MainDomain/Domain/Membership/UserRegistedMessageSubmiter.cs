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
using DotPay.MainDomain.Repository;
using DotPay.Tools.DistributedMessageSender;

namespace DotPay.MainDomain
{
    /*
     [Component]
     [AwaitCommitted]
     public class UserRegistedMessageSubmiter : IEventHandler<UserRegisted>     //用户注册成功
     {
         public void Handle(UserRegisted @event)
         {
             var msg = new UserReisterMessage(@event.RegistUser.ID, @event.RippleAddress);

             var exchangeName = Utilities.GetExchangeAndQueueNameOfUserRegistered().Item1;

             var msgBytes = Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(msg));
             var existKey = "userRegistered_" + @event.RegistUser.ID;
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
                 Log.Error("用户注册后，发送注册消息时，出现错误", @event.RegistUser.ID, ex.Message);
             }
         }

         public class UserReisterMessage
         {
             public UserReisterMessage(int userID, string rippleAddress)
             {
                 this.UserID = userID;
                 this.RippleAddress = rippleAddress;
             }
             public int UserID { get; set; }
             public string RippleAddress { get; set; }
         }
     }*/
}
