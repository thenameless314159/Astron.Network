using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Astron.Network.Buffers;
using Astron.Network.Tests.Helpers;

using Xunit;

namespace Astron.Network.Tests.Buffers
{
    public class SequenceReaderTests
    {
        private const int _firstValue = 150;
        private const int _secondValue = 178;

        [Fact]
        public void TryRead_ShouldReturnTrue_OnSegmentComplete()
        {
            var input = new ReadOnlySequence<byte>(_createBuffer());

            var reader = new SequenceReader(input);
            reader.TryRead(BinaryPrimitives.ReadInt32LittleEndian, out var firstValue);
            reader.TryRead(BinaryPrimitives.ReadInt32LittleEndian, out var secondValue);

            Assert.Equal(_firstValue, firstValue);
            Assert.Equal(_secondValue, secondValue);
        }

        [Fact]
        public void TryRead_ShouldReturnTrue_OnSegmentSplitted()
        {
            var buffer = _createBuffer();
            var firstSegment = new SequenceSegment(buffer.Slice(0, 4));
            var secondSegment = firstSegment.Add(buffer.Slice(4, 4));

            var input = new ReadOnlySequence<byte>(
                firstSegment, 0,
                secondSegment, 4);

            var reader = new SequenceReader(input);
            reader.TryRead(BinaryPrimitives.ReadInt32LittleEndian, out var firstValue);
            reader.TryRead(BinaryPrimitives.ReadInt32LittleEndian, out var secondValue);

            Assert.Equal(_firstValue, firstValue);
            Assert.Equal(_secondValue, secondValue);
        }

        [Fact]
        public void TryRead_ShouldReturnFalse_OnSegmentIncomplete()
        {
            var input = new ReadOnlySequence<byte>(_createBuffer().Slice(0, 3));

            var reader = new SequenceReader(input);
            Assert.False(reader.TryRead(BinaryPrimitives.ReadInt32LittleEndian, out var firstValue));
        }

        private ReadOnlyMemory<byte> _createBuffer()
        {
            Memory<byte> buffer = new byte[8];
            BinaryPrimitives.WriteInt32LittleEndian(buffer.Span, _firstValue);
            BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(4).Span, _secondValue);
            return buffer;
        }
    }
}
