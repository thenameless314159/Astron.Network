﻿using System.Threading.Tasks;

namespace Astron.Network.Abstractions
{
    public interface IRemoteClient : ISocketClient
    {
        Task ConnectAsync(string host, int port);
    }
}
