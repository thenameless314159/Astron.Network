using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

using Astron.Network.Abstractions;
using Astron.Network.Tests.Helpers;
using Astron.Network.Tests.Mocks;

using Xunit;

namespace Astron.Network.Tests
{
    public class SocketManagerTests
    {
        private readonly ISocketManager _socketManager;
        private readonly ISocketCluster _socketCluster;
        private readonly MockSocketProxy _listener;

        public SocketManagerTests()
        {
            _listener = new MockSocketProxy();
            _socketCluster = new SocketCluster(1000);
            _socketManager = new SocketManager(_listener, _socketCluster);
        }

        [Fact]
        public async Task CreateClient_ShouldAcceptIncomingClient()
        {
            var client = await _socketManager.CreateClient();
            _listener.VerifyAccept();
            Assert.NotNull(client);
        }

        [Fact]
        public async Task CreateClient_ShouldAddToCluster()
        {
            var client = await _socketManager.CreateClient();
            var clusterClient = _socketCluster.GetSnapshot().First();
            Assert.Equal(client, clusterClient);
        }

        [Fact]
        public async Task DestroyClient_ShouldDisposeSpecifiedClient()
        {
            var client = await _socketManager.CreateClient();
            _socketManager.DestroyClient(client);
            var mock = (MockSocketProxy)client;
            mock.VerifyDispose();
        }

        [Fact]
        public async Task DestroyClient_ShouldRemoveFromCluster()
        {
            var client = await _socketManager.CreateClient();
            _socketManager.DestroyClient(client);
            var snapshot = _socketCluster.GetSnapshot();
            Assert.DoesNotContain(client, snapshot);
        }

        [Fact]
        public async Task BroadcastAsync_ShouldSendToAll()
        {
            Memory<byte> buffer = new byte[8];
            BinaryPrimitives.WriteInt32LittleEndian(buffer.Span, 123);
            BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(4).Span, 456);

            var clients = new List<MockSocketProxy>(5);
            for (var i = 0; i < 5; i++) clients.Add((MockSocketProxy)await _socketManager.CreateClient());

            await _socketManager.BroadcastAsync(buffer, SocketFlags.None);
            clients.VerifyAll(m => m.VerifySendAsync(buffer, SocketFlags.None));
        }

        [Fact]
        public async Task BroadcastAsync_ShouldHandleDisposedClient()
        {
            var client = await _socketManager.CreateClient();
            client.Dispose();
            Assert.Throws<ObjectDisposedException>(() =>
                client.SendAsync(new ReadOnlyMemory<byte>(), SocketFlags.None));

            // should not throw
            await _socketManager.BroadcastAsync(new ReadOnlyMemory<byte>(), SocketFlags.None);
            ((MockSocketProxy)client).VerifyNeverSentAsync(new ReadOnlyMemory<byte>(), SocketFlags.None);
        }
    }
}
