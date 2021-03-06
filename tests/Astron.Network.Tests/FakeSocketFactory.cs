﻿using Astron.Network.Abstractions;
using Astron.Network.Tests.Mocks;

namespace Astron.Network.Tests
{
    public class FakeSocketFactory : SocketFactory
    {
        protected override ISocketProxy CreateSocket() => new MockSocketProxy();
    }
}
