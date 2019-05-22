using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Astron.Network.Abstractions
{
    public interface ISocketClient : IDisposable
    {
        EndPoint RemoteEndPoint { get; }
        ValueTask<int> ReceiveAsync(Memory<byte> buffer, SocketFlags flags, CancellationToken token = default);
        ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, SocketFlags flags, CancellationToken token = default);
    }
}
