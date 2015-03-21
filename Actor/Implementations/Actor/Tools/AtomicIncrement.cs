using System.Threading.Tasks; 
using Dotpay.Actor.Tools.Interfaces;
using Orleans;
using Orleans.Providers;

namespace Dotpay.Actor.Tools.Implementations
{
    /// <summary>
    /// 有连续顺序的序号、原子的序号生成器
    /// <remarks>为了保证数字的连续性，需要借助持久化，所以此序号生成器的性能不会太高，需注意使用场景</remarks>
    /// </summary>
    [StorageProvider(ProviderName = "CouchbaseStore")]
    public class AtomicIncrement : Grain<IAtomicIncrementState>, IAtomicIncrement
    {
        async Task<int> IAtomicIncrement.GetNext()
        {
            this.State.Seed += 1;
            await this.State.WriteStateAsync();
            return this.State.Seed;
        }
    }

    public interface IAtomicIncrementState : IGrainState
    {
        int Seed { get; set; }
    }
}
