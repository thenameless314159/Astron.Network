using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Astron.Network.Abstractions;

using Moq;

namespace Astron.Network.Tests.Mocks
{
    public class MockSocketProxy : ISocketProxy
    {
        private readonly Mock<ISocketProxy> _mock;
        private bool _disposed;

        public EndPoint RemoteEndPoint => _mock.Object.RemoteEndPoint;

        public MockSocketProxy() => _mock = new Mock<ISocketProxy>();

        public void Bind(EndPoint endPoint) => _mock.Object.Bind(endPoint);
        public void Listen(int backlog) => _mock.Object.Listen(backlog);

        public Task<ISocketClient> AcceptAsync() => Task.FromResult<ISocketClient>(new MockSocketProxy());

        public Task ConnectAsync(string host, int port) => _mock.Object.ConnectAsync(host, port);

        public ValueTask<int> ReceiveAsync(Memory<byte> buffer, SocketFlags flags, CancellationToken token = default)
            => _mock.Object.ReceiveAsync(buffer, flags, token);

        public ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, SocketFlags flags,
            CancellationToken token = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MockSocketProxy));

            return _mock.Object.SendAsync(buffer, flags, token);
        }

        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MockSocketProxy));

            _mock.Object.Dispose();
            _disposed = true;
        }

        public void VerifyBind(EndPoint endPoint) 
            => _mock.Verify(m => m.Bind(endPoint));
        public void VerifyListen(int backlog) 
            => _mock.Verify(m => m.Listen(backlog));
        public void VerifyAccept() 
            => _mock.Verify(m => m.AcceptAsync());
        public void VerifyConnectAsync(string host, int port) 
            => _mock.Verify(m => m.ConnectAsync(host, port));

        public void VerifySendAsync(ReadOnlyMemory<byte> buffer, SocketFlags flags,
            CancellationToken token = default) 
            => _mock.Verify(m => m.SendAsync(buffer, flags, token));

        public void VerifyNeverSentAsync(ReadOnlyMemory<byte> buffer, SocketFlags flags,
            CancellationToken token = default) 
            => _mock.Verify(m => m.SendAsync(buffer, flags, token), Times.Never);
        public void VerifyDispose() 
            => _mock.Verify(m => m.Dispose());
    }
}
