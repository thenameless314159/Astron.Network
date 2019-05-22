using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Astron.Network.Abstractions
{
    public interface ISocketFactory
    {
        ISocketListener CreateListener(EndPoint endPoint, int backlog);
        Task<ISocketClient> CreateRemote(string host, int port);
    }
}
