using System;
using System.Collections.Generic;
using System.Text;

using Astron.Network.Abstractions;
using Astron.Network.Threading;

namespace Astron.Network
{
    public class SocketCluster : SafeList<ISocketClient>, ISocketCluster
    {
        public SocketCluster(int capacity) : base(capacity)
        {
        }
    }
}
