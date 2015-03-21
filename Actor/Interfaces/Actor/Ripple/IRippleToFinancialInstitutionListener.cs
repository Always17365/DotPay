 
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace Dotpay.Actor.Interfaces.Ripple
{
    public interface IRippleToFinancialInstitutionListener : Orleans.IGrainWithIntegerKey
    {
        Task Start();
        Task Receive(MqMessage message);
    }
}
