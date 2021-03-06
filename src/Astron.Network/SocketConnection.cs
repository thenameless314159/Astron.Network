﻿using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

using Astron.Network.Abstractions;
using Astron.Network.Framing;

namespace Astron.Network
{
    public abstract class SocketConnection<TMeta> : ISocketConnection<TMeta>
        where TMeta : class, IMessageMetadata
    {
        public IDuplexPipe Pipe { private get; set; }
        public FrameParser<TMeta> Parser { private get; set; }

        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _token;

        protected SocketConnection()
        {
            _cts = new CancellationTokenSource();
            _token = _cts.Token;
        }

        public async Task Setup()
        {
            var reader = Pipe?.Input ?? throw new ObjectDisposedException(ToString());

            try
            {
                await OnCreateAsync();
                var makingProgress = false;
                while (!_token.IsCancellationRequested)
                {
                    if (!(makingProgress && reader.TryRead(out var readResult)))
                        readResult = await reader.ReadAsync(_token);
                    if (readResult.IsCanceled) break;

                    var buffer = readResult.Buffer;

                    makingProgress = false;
                    while (Parser.TryParseFrame(ref buffer, out var frame))
                    {
                        makingProgress = true;
                        await OnReceiveAsync((Frame<TMeta>)frame);
                    }

                    reader.AdvanceTo(buffer.Start, buffer.End);

                    if (!makingProgress && readResult.IsCompleted) break;
                }
                try { reader.Complete(); } catch { }
            }
            catch (Exception ex)
            {
                try { reader.Complete(ex); } catch { }
            }
            finally
            {
                try { await OnDestroyAsync(); } catch { }
            }
        }

        public ValueTask<FlushResult> SendAsync(Frame<TMeta> frame)
        {
            var writer = Pipe?.Output ?? throw new ObjectDisposedException(ToString());
            var metaLength = Parser.GetMetadataLength(frame.Metadata);
            var span = writer.GetSpan(metaLength);
            Parser.WriteMetadata(span, frame.Metadata);
            writer.Advance(metaLength);

            return frame.Meta.Length == 0 
                ? writer.FlushAsync(_token) 
                : writer.WriteAsync(frame.Payload, _token);
        }

        protected virtual ValueTask OnCreateAsync() => default;
        protected virtual ValueTask OnDestroyAsync() => default;
        protected abstract ValueTask OnReceiveAsync(Frame<TMeta> frame);

        public void Dispose()
        {
            _cts.Cancel();
            Pipe?.Input.Complete();
            Pipe?.Output.Complete();
            Pipe = null;
        }
    }
}
