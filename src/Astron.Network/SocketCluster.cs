using Astron.Core.Threading;
using Astron.Network.Abstractions;

namespace Astron.Network
{
    public class SocketCluster : SafeList<ISocketClient>, ISocketCluster
    {
        public SocketCluster(int capacity) : base(capacity)
        {
        }
    }
}
