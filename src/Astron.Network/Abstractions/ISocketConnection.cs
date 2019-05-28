using System;
using System.IO.Pipelines;
using System.Threading.Tasks;

using Astron.Core;
using Astron.Network.Framing;

namespace Astron.Network.Abstractions
{
    public interface ISocketConnection<TMeta> : IAsyncSetup, IDisposable
        where TMeta : IMessageMetadata
    {
        IDuplexPipe Pipe { set; }
        FrameParser<TMeta> Parser { set; }

        ValueTask<FlushResult> SendAsync(Frame<TMeta> frame);
    }
}
