using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Actor.Interfaces;
﻿using Orleans;

namespace Dotpay.Actor.Service.Interfaces
{
    public interface IRefundTransactionManager : Orleans.IGrainWithIntegerKey
    {
        Task Start();
        Task Receive(MqMessage message);
    }
}
