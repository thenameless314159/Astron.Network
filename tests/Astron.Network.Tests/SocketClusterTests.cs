using System.Linq;

using Astron.Network.Abstractions;
using Astron.Network.Tests.Mocks;

using Xunit;

namespace Astron.Network.Tests
{
    public class SocketClusterTests
    {
        private readonly ISocketCluster _cluster;

        public SocketClusterTests() => _cluster = new SocketCluster(1000);

        [Fact]
        public void Ctor__ShouldCreateList()
        {
            Assert.Empty(_cluster.GetSnapshot());
        }

        [Fact]
        public void Add__ShouldAppendList()
        {
            var socket = new MockSocketProxy();
            _cluster.Add(socket);
            var sockets = _cluster.GetSnapshot();
            Assert.Single(sockets);
            Assert.Equal(socket, sockets.First());
        }

        [Fact]
        public void Remove_ShouldDeleteSpecified()
        {
            _addClients(1);

            var client = _cluster.GetSnapshot().First();
            _cluster.Remove(client);

            Assert.Empty(_cluster.GetSnapshot());
        }

        [Fact]
        public void Clear_ShouldRemoveAll()
        {
            _addClients(5);
            Assert.Equal(5, _cluster.GetSnapshot().Count);
            _cluster.Clear();
            Assert.Empty(_cluster.GetSnapshot());
        }


        private void _addClients(int n = 5)
        {
            var sockets = new ISocketProxy[n];
            for (var i = 0; i < sockets.Length; i++)
                sockets[i] = new MockSocketProxy();

            foreach (var socket in sockets)
            {
                _cluster.Add(socket);
            }
        }
    }
}
