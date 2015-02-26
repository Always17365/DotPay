using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Common.Enum;
using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Interfaces.Tools
{
    public interface ISequenceNoGenerator : Orleans.IGrainWithIntegerKey
    {
        Task<Immutable<string>> GetNext();
    }
}
