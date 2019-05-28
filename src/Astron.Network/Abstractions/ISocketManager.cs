using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Astron.Network.Abstractions
{
    public interface ISocketManager : IDisposable
    {
        Task<ISocketClient> CreateClient();
        void DestroyClient(ISocketClient client);
        Task BroadcastAsync(ReadOnlyMemory<byte> buffer, SocketFlags flags, CancellationToken token = default);
    }
}
