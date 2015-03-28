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
        async Task IAtomicIncrement.SetSeed(int seed)
        {
            if (this.State.Seed == 0)
            {
                this.State.Seed = seed;
                await this.State.WriteStateAsync();
            }
        }

        Task<int> IAtomicIncrement.GetNext()
        {
            this.State.Seed += 1;
            //为了可靠性，每次写入一次.为了性能，不await
            this.State.WriteStateAsync();
            return Task.FromResult(this.State.Seed);
        }
    }

    public interface IAtomicIncrementState : IGrainState
    {
        int Seed { get; set; }
    }
}
