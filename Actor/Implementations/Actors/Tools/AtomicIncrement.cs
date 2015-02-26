using System.Threading.Tasks;
using Dotpay.Actor.Interfaces;
using Dotpay.Actor.Interfaces.Tools;
using Orleans;
using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
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
