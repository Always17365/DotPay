 
﻿using System.Threading.Tasks;
﻿using Orleans;

namespace Dotpay.Actor.Tools.Interfaces
{
    /// <summary>
    /// 自增长Id生成器
    /// <remarks>目前主要用于Ripple中的DestinationTag的生成，暂无其它用途</remarks>
    /// </summary>
    public interface IAtomicIncrement : IGrainWithStringKey
    {
        Task SetSeed(int seed);
        Task<int> GetNext();
    }
}
