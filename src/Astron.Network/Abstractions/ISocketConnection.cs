using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

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
