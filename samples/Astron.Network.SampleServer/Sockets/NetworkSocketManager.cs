using System.Net;

namespace Astron.Network.SampleServer.Sockets
{
    public class NetworkSocketManager : SocketManager
    {
        public NetworkSocketManager(int backlog, int capacity) 
            : base(backlog, new IPEndPoint(IPAddress.Loopback, 8080), new TcpSocketFactory(), 
                new SocketCluster(capacity))
        {
        }
    }
}
