using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Astron.Network.Abstractions;

using Pipelines.Sockets.Unofficial;

namespace Astron.Network
{
    public sealed class SocketProxy : ISocketProxy
    {
        private Socket _socket;

        public SocketProxy(Socket s) => _socket = s ?? throw new ArgumentNullException(nameof(s));

        public void Bind(EndPoint endPoint) => _socket.Bind(endPoint);

        public void Listen(int backlog) => _socket.Listen(backlog);

        public async Task<ISocketClient> AcceptAsync()
            => new SocketProxy(await _socket.AcceptAsync());

        public ValueTask<int> ReceiveAsync(Memory<byte> buffer, SocketFlags flags, CancellationToken token = default)
            => _socket.ReceiveAsync(buffer, flags, token);

        public ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, SocketFlags flags, CancellationToken token = default)
            => _socket.SendAsync(buffer, flags, token);

        public Task ConnectAsync(string host, int port) => _socket.ConnectAsync(host, port);

        public EndPoint RemoteEndPoint => _socket.RemoteEndPoint;

        public IDuplexPipe CreatePipe() => SocketConnection.Create(_socket);

        public void Dispose()
        {
            _socket?.Dispose();
            _socket = null;
        }
    }
}
