using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

using Astron.Network.Abstractions;
using Astron.Network.Framing;
using Astron.Network.Tests.Framing;
using Astron.Network.Tests.Mocks;

namespace Astron.Network.Tests
{
    public class FakeSocketServer : SocketServer<FakeConnection, MessageMetadata>
    {
        public Action<FakeConnection> OnAccepted;

        public FakeSocketServer(ISocketCluster cluster, Action<Exception> onError = default) 
            : base(new SocketManager(new MockSocketProxy(), cluster), 
                new FakeFrameParser(), 
                onError)
        {
        }

        protected override IDuplexPipe CreatePipe(ISocketClient socket) => new FakeDuplexPipe();

        protected override ValueTask OnClientAccepted(FakeConnection client)
        {
            OnAccepted?.Invoke(client);
            return default;
        }
    }
}
