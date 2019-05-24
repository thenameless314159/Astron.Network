using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using Astron.Network.Abstractions;

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
