using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

using Astron.Network.Abstractions;

namespace Astron.Network
{
    public class TcpSocketFactory : SocketFactory
    {
        protected override ISocketProxy CreateSocket()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            return new SocketProxy(socket);
        }
    }
}
