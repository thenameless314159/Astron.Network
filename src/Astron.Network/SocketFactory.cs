using System.Net;
using System.Threading.Tasks;

using Astron.Network.Abstractions;

namespace Astron.Network
{
    public abstract class SocketFactory
    {
        protected abstract ISocketProxy CreateSocket();

        public ISocketListener CreateListenerSocket(EndPoint endPoint, int backlog)
        {
            var socket = CreateSocket();
            socket.Bind(endPoint);
            socket.Listen(backlog);
            return socket;
        }

        public async Task<ISocketClient> CreateRemoteSocket(string host, int port)
        {
            var socket = CreateSocket();
            await socket.ConnectAsync(host, port).ConfigureAwait(false);
            return socket;
        }
    }
}
