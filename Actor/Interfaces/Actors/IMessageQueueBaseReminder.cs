using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor.Interfaces.Actors
{
    interface IMessageQueueBaseReminder
    {
        Task ReceiveMessageReminder<T>(Immutable<T> message);
    }
}
