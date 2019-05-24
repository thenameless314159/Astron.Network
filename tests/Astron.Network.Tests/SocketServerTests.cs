using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Astron.Network.Abstractions;

using Xunit;

namespace Astron.Network.Tests
{
    public class SocketServerTests
    {
        private FakeSocketServer _socketServer;
        private ISocketCluster _cluster;

        public SocketServerTests()
        {
            _cluster = new SocketCluster(1000);
            _socketServer = new FakeSocketServer(_cluster);
        }

        [Fact]
        public async Task Setup_ShouldCreateConnection_OnAccepted()
        {
            _socketServer.OnAccepted = client =>
            {
                Assert.NotNull(client);
                _socketServer.Dispose(); // close server
            };

            await _socketServer.Setup();
        }

        [Fact]
        public async Task Setup_ShouldAddToCluster_OnAccepted()
        {
            _socketServer.OnAccepted = client =>
            {
                Assert.NotEmpty(_cluster.GetSnapshot());
                _socketServer.Dispose(); // close server
            };

            Assert.Empty(_cluster.GetSnapshot());
            await _socketServer.Setup();
        }

        [Fact]
        public async Task Setup_ShouldRemoveFromCluster_OnDisposed()
        {
            _socketServer.OnAccepted = client =>
            {
                Assert.NotEmpty(_cluster.GetSnapshot());
                _socketServer.Dispose(); // close server
            };

            Assert.Empty(_cluster.GetSnapshot());
            await _socketServer.Setup();
            Assert.Empty(_cluster.GetSnapshot());
        }

        [Fact]
        public async Task Setup_ShouldRemoveFromCluster_OnDisconnected()
        {
            var count = 2;
            FakeConnection client = default;
            _socketServer.OnAccepted = c =>
            {
                switch (count)
                {
                    case 2:
                        count--;
                        client = c;
                        return;
                    case 1:
                        count--;
                        client?.Dispose();
                        Thread.Sleep(10); // ensure not called before client removed from cluster
                        return;
                    case 0:
                        Assert.Equal(2, _cluster.GetSnapshot().Count);
                        _socketServer.Dispose(); // close server
                        break;
                }
            };

            await _socketServer.Setup();
        }
    }
}
