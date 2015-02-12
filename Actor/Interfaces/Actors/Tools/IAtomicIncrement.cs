 
﻿using System.Threading.Tasks;
﻿using Orleans;

namespace Dotpay.Actor.Interfaces
{
    public interface IAtomicIncrement : IGrainWithStringKey
    {
        Task<int> GetNext();
    }
}
