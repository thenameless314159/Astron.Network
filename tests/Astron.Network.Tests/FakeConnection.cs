using System;
using System.Threading.Tasks;

using Astron.Network.Framing;

using Moq;

namespace Astron.Network.Tests
{
    public class FakeConnection : SocketConnection<MessageMetadata>
    {
        private readonly Mock<FakeConnection> _mock;

        public FakeConnection() => _mock = new Mock<FakeConnection>();

        public Action<MessageMetadata> OnReceive;
        public Action OnCreate;

        protected override ValueTask OnReceiveAsync(Frame<MessageMetadata> frame)
        {
            OnReceive?.Invoke(frame.Metadata);
            return default;
        }

        protected override ValueTask OnCreateAsync()
        {
            _mock.Object.OnCreateAsync();
            OnCreate?.Invoke();
            return base.OnCreateAsync();
        }

        protected override ValueTask OnDestroyAsync()
        {
            _mock.Object.OnDestroyAsync();
            return base.OnCreateAsync();
        }

        public void VerifyOnCreate() => _mock.Verify(m => m.OnCreateAsync());
        public void VerifyOnDestroy() => _mock.Verify(m => m.OnDestroyAsync());
    }
}
