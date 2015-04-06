using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Common.Enum;
using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Tools
{
    public interface ISequenceNoGenerator : Orleans.IGrainWithIntegerKey
    {
        Task<string> GetNext();
    }
}
