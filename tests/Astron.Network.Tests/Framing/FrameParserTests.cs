using System;
using System.Buffers;
using System.Buffers.Binary;

using Astron.Network.Tests.Helpers;

using Xunit;

namespace Astron.Network.Tests.Framing
{
    public class FrameParserTests
    {
        private readonly FakeFrameParser _frameParser;

        public FrameParserTests() => _frameParser = new FakeFrameParser();

        /// <summary>
        /// Case: when you receive a frame with a complete message
        /// </summary>
        [Fact]
        public void TryParseFrame_ShouldReturnTrue_OnCompleteMessage()
        {
            var fullFrame = new ReadOnlySequence<byte>(_createMessage(150));
            Assert.True(_frameParser.TryParseFrame(ref fullFrame, out var frame));
        }

        /// <summary>
        /// Case: when you receive a frame with a complete message.
        /// </summary>
        [Fact]
        public void TryParseFrame_ShouldReturnMetadata_OnCompleteMessage()
        {
            var fullFrame = new ReadOnlySequence<byte>(_createMessage(150));
            _frameParser.TryParseFrame(ref fullFrame, out var frame);

            Assert.NotNull(frame.Meta);
            Assert.Equal(4, frame.Meta.Length);
        }

        /// <summary>
        /// Case: when you receive a frame without enough byte to parse the metadata.
        /// </summary>
        [Fact]
        public void TryParseFrame_ShouldReturnFalse_OnIncompleteMetadata()
        {
            var fullFrame = new ReadOnlySequence<byte>(_createMessage(150).Slice(0, 3));
            Assert.False(_frameParser.TryParseFrame(ref fullFrame, out var frame));
        }
        
        /// <summary>
        /// Case: when you receive a frame without enough byte to parse the metadata.
        /// </summary>
        [Fact]
        public void TryParseFrame_ShouldNotReturnFrame_OnIncompleteMetadata()
        {
            var fullFrame = new ReadOnlySequence<byte>(_createMessage(150).Slice(0, 3));
            _frameParser.TryParseFrame(ref fullFrame, out var frame);
            Assert.Null(frame);
        }
        
        /// <summary>
        /// Case: when you receive a frame without enough byte to get the payload.
        /// </summary>
        [Fact]
        public void TryParseFrame_ShouldReturnFalse_OnIncompletePayload()
        {
            var fullFrame = new ReadOnlySequence<byte>(_createMessage(150).Slice(0, 6));
            Assert.False(_frameParser.TryParseFrame(ref fullFrame, out var frame));
        }
        
        /// <summary>
        /// Case: when you receive a frame without enough byte to get the payload.
        /// </summary>
        [Fact]
        public void TryParseFrame_ShouldNotReturnFrame_OnIncompletePayload()
        {
            var fullFrame = new ReadOnlySequence<byte>(_createMessage(150).Slice(0, 6));
            _frameParser.TryParseFrame(ref fullFrame, out var frame);
            Assert.Null(frame);
        }

        /// <summary>
        /// Case: when you receive two complete message in a single frame.
        /// </summary>
        [Fact]
        public void TryParseFrame_ShouldReturnTrue_OnTwoCompleteMessage()
        {
            var firstFrame    = _createMessage(150);
            var secondFrame   = _createMessage(160);
            var firstSegment  = new SequenceSegment(firstFrame);
            var secondSegment = firstSegment.Add(secondFrame);

            var fullFrame = new ReadOnlySequence<byte>(
                firstSegment, 0,
                secondSegment, 8);

            Assert.True(_frameParser.TryParseFrame(ref fullFrame, out var firstMessage));
            Assert.True(_frameParser.TryParseFrame(ref fullFrame, out var secondMessage));
        }

        /// <summary>
        /// Case: when you receive one complete message and a half in a single frame.
        /// </summary>
        [Fact]
        public void TryParseFrame_ShouldReturnFalse_OnCompleteMessageAndHalf()
        {
            var firstFrame    = _createMessage(150);
            var secondFrame   = _createMessage(160);
            var firstSegment  = new SequenceSegment(firstFrame);
            var secondSegment = firstSegment.Add(secondFrame.Slice(4));

            var fullFrame = new ReadOnlySequence<byte>(
                firstSegment, 0,
                secondSegment, 4);

            Assert.True(_frameParser.TryParseFrame(ref fullFrame, out var firstMessage));
            Assert.False(_frameParser.TryParseFrame(ref fullFrame, out var secondMessage));
        }
        private ReadOnlyMemory<byte> _createMessage(int value)
        {
            Memory<byte> buffer = new byte[8];
            BinaryPrimitives.WriteInt32LittleEndian(buffer.Span, sizeof(int));
            BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(4).Span, value);
            return buffer;
        }
    }
}
