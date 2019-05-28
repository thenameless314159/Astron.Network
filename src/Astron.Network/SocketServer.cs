using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

using Astron.Core;
using Astron.Network.Abstractions;
using Astron.Network.Framing;


namespace Astron.Network
{
    public abstract class SocketServer<TConnection, TMeta> : IAsyncSetup
        where TConnection : ISocketConnection<TMeta>, new()
        where TMeta : IMessageMetadata
    {
        private ISocketManager _socketManager;
        private readonly CancellationTokenSource _cts;
        private readonly FrameParser<TMeta> _frameParser;
        private readonly Action<Exception>  _onError;

        protected SocketServer(ISocketManager socketManager, FrameParser<TMeta> frameParser,
            Action<Exception> onError = default)
        {
            _cts = new CancellationTokenSource();
            _onError = onError;
            _socketManager = socketManager;
            _frameParser = frameParser ?? throw new ArgumentNullException(nameof(frameParser));
        }

        public async Task Setup()
        {
            while (!_cts.IsCancellationRequested)
            {
                var client = await _socketManager.CreateClient();
                var pipe = CreatePipe(client);
                var connection = new TConnection { Pipe = pipe, Parser = _frameParser };

                await OnClientAccepted(connection);
                _ = connection.Setup()
                    .ContinueWith(t => _onError(t.Exception?.InnerException),
                        TaskContinuationOptions.OnlyOnFaulted)
                    .ContinueWith(t => _socketManager.DestroyClient(client),
                        TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _socketManager?.Dispose();
            _socketManager = null;
        }

        protected abstract IDuplexPipe CreatePipe(ISocketClient socket);
        protected virtual ValueTask OnClientAccepted(TConnection client) => default;
    }
}
