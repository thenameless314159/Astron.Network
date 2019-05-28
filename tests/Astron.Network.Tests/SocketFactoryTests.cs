using System.Net;
using System.Threading.Tasks;

using Astron.Network.Tests.Mocks;

using Xunit;

namespace Astron.Network.Tests
{
    public class SocketFactoryTests
    {
        private readonly EndPoint _endPoint = new IPEndPoint(IPAddress.Loopback, _port);
        private readonly string _host = IPAddress.Loopback.ToString();
        private const int _backlog = 100;
        private const int _port = 8080;

        private SocketFactory _factory;

        public SocketFactoryTests() => _factory = new FakeSocketFactory();

        [Fact]
        public void CreateListener_ShouldBindAndListen()
        {
            var mock = (MockSocketProxy)_factory.CreateListenerSocket(_endPoint, _backlog);
            mock.VerifyBind(_endPoint);
            mock.VerifyListen(_backlog);
        }

        [Fact]
        public async Task CreateRemote_ShouldConnect()
        {
            var socket = (MockSocketProxy)await _factory.CreateRemoteSocket(_host, _port);
            socket.VerifyConnectAsync(_host, _port);
        }
    }
}
