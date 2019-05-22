using System;
using System.Collections.Generic;
using System.Text;

using Astron.Network.Threading;

namespace Astron.Network.Abstractions
{
    public interface ISocketCluster : ISafeCollection<ISocketClient>
    {
    }
}
