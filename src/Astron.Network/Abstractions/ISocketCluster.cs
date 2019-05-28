using Astron.Core.Threading;

namespace Astron.Network.Abstractions
{
    public interface ISocketCluster : ISafeCollection<ISocketClient>
    {
    }
}
