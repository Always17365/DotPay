using System.Threading.Tasks;
using Orleans;

namespace Dotpay.Actor.Tools
{
    public interface ISequenceNoGenerator : IGrainWithIntegerKey
    {
        Task<string> GetNext();
    }
}
