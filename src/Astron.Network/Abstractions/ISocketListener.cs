using System;
using System.Net;
using System.Threading.Tasks;

namespace Astron.Network.Abstractions
{
    public interface ISocketListener : IDisposable
    {
        void Bind(EndPoint endPoint);
        void Listen(int backlog);
        Task<ISocketClient> AcceptAsync();
    }
}
