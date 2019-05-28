using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading.Tasks;

using Astron.Network.Framing;
using Astron.Network.Tests.Framing;

using Xunit;

namespace Astron.Network.Tests
{
    public class SocketConnectionTests
    {
        [Fact]
        public async Task Setup_ShouldInvokeOnCreateAsync__OnReceiveLoopStarted()
        {
            var pipe = new FakeDuplexPipe();
            var conn = new FakeConnection { Pipe = pipe, Parser = new FakeFrameParser() };
            conn.OnCreate = () => conn.Dispose();

            await conn.Setup();
            conn.VerifyOnCreate();
        }

        [Fact]
        public async Task Setup_ShouldInvokeOnDestroyAsync_OnReceiveLoopEnded()
        {
            var pipe = new FakeDuplexPipe();
            var conn = new FakeConnection { Pipe = pipe, Parser = new FakeFrameParser() };
            conn.OnCreate = () => conn.Dispose();

            await conn.Setup();
            conn.VerifyOnDestroy();
        }


        [Fact]
        public async Task Setup_ShouldReadFrame_OnReceivedData()
        {
            var pipe = new FakeDuplexPipe();
            var conn = new FakeConnection { Pipe = pipe, Parser = new FakeFrameParser()};
            var setup = conn.Setup();

            conn.OnReceive = metadata =>
            {
                Assert.NotNull(metadata);
                Assert.Equal(4, metadata.Length);
                conn.Dispose();
            };

            //let's write a simple message.
            await pipe.FakeRead(_createMessage(4));

            // then start the receive loop
            await setup;
        }

        [Fact]
        public async Task SendAsync_ShouldWriteToPipeOutput_OnEmptyPayload()
        {
            var pipe  = new FakeDuplexPipe();
            var conn  = new FakeConnection { Pipe = pipe, Parser = new FakeFrameParser() };

            await conn.SendAsync(new Frame<MessageMetadata>(new ReadOnlySequence<byte>(new byte[0]),
                new MessageMetadata {Length = 0}));

            var sentBuffer = await pipe.ReadPipeWriter();
            Assert.Equal(4, sentBuffer.Buffer.Length);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(64)]
        [InlineData(128)]
        public async Task SendAsync_ShouldWriteToPipeOutput_OnPayload(int length)
        {
            var pipe = new FakeDuplexPipe();
            var conn = new FakeConnection { Pipe = pipe, Parser = new FakeFrameParser() };

            await conn.SendAsync(new Frame<MessageMetadata>(new byte[length],
                new MessageMetadata { Length =  length}));

            var sentBuffer = await pipe.ReadPipeWriter();
            Assert.Equal(length + 4, sentBuffer.Buffer.Length);
        }

        [Fact]
        public async Task SendAsync_ShouldThrow_OnDisposed()
        {
            var pipe = new FakeDuplexPipe();
            var conn = new FakeConnection { Pipe = pipe, Parser = new FakeFrameParser() };
            conn.OnCreate = () => conn.Dispose();

            await conn.Setup();
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await conn.SendAsync(
                new Frame<MessageMetadata>(new ReadOnlySequence<byte>(new byte[0]),
                    new MessageMetadata {Length = 0})));
        }

        [Fact]
        public async Task Setup_ShouldThrow_OnDisposed()
        {
            var pipe = new FakeDuplexPipe();
            var conn = new FakeConnection { Pipe = pipe, Parser = new FakeFrameParser() };
            conn.OnCreate = () => conn.Dispose();

            await conn.Setup();
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await conn.Setup());
        }

        private static byte[] _createMessage(int length)
        {
            var buffer = new byte[length + 4];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, length);
            return buffer;
        }
    }
}
