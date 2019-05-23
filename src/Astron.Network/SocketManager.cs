using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Astron.Network.Abstractions;

namespace Astron.Network
{
    public class SocketManager : ISocketManager
    {
        private ISocketListener _listener;
        private ISocketCluster  _cluster;

        public SocketManager(int backlog, EndPoint endPoint, SocketFactory sf, ISocketCluster mainCluster)
        {
            _listener = sf?.CreateListenerSocket(endPoint, backlog) ?? throw new ArgumentNullException(nameof(sf));
            _cluster  = mainCluster ?? throw new ArgumentNullException(nameof(mainCluster));
        }

        public SocketManager(ISocketListener socketListener, ISocketCluster mainCluster)
        {
            _listener = socketListener ?? throw new ArgumentNullException(nameof(socketListener));
            _cluster  = mainCluster ?? throw new ArgumentNullException(nameof(mainCluster));
        }

        public virtual bool CanAccept(ISocketClient socket) => true;

        public async Task<ISocketClient> CreateClient()
        {
            var socket = await _listener.AcceptAsync();
            if (!CanAccept(socket))
            {
                socket.Dispose();
                return null;
            }
            _cluster.Add(socket);
            return socket;
        }

        public void DestroyClient(ISocketClient socket)
        {
            _cluster.Remove(socket);
            socket.Dispose();
        }

        public async Task BroadcastAsync(ReadOnlyMemory<byte> buffer, SocketFlags flags, CancellationToken token = default)
        {
            var clients = _cluster.GetSnapshot();
            foreach (var client in clients)
                try { await client.SendAsync(buffer, flags, token); } catch { }
        }

        public void Dispose()
        {
            _listener?.Dispose();
            _listener = null;

            var clients = _cluster.GetSnapshot();
            foreach (var client in clients) client.Dispose();
            _cluster.Clear();
            _cluster = null;
        }
    }
}
