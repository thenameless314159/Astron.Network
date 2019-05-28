using System;
using System.IO.Pipelines;
using System.Threading.Tasks;

using Astron.Network.Abstractions;
using Astron.Network.SampleProtocol.Framing;
using Astron.Network.SampleProtocol.Messages;

namespace Astron.Network.SampleServer.Sockets
{
    public class NetworkServer : SocketServer<NetworkConnection, NetworkMetadata>
    {
        public NetworkServer(int backlog, int capacity) : base(new NetworkSocketManager(backlog, capacity), new NetworkFrameParser(), e => Console.WriteLine(e))
        {
            Console.WriteLine("Network server started at 127.0.0.1:8080, waiting for incoming connections...");
        }

        protected override IDuplexPipe CreatePipe(ISocketClient socket)
        {
            Console.WriteLine($"New client at {socket.RemoteEndPoint} !");
            return ((SocketProxy)socket).CreatePipe();
        }

        protected override async ValueTask OnClientAccepted(NetworkConnection client)
        {
            await client.SendMessageAsync(new PingMessage(true));
        }
    }
}
