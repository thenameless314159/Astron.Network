using System;
using System.Buffers;
using System.Buffers.Binary;

using Astron.Network.Buffers;
using Astron.Network.Framing;

namespace Astron.Network.Tests.Framing
{
    public class FakeFrameParser : FrameParser<MessageMetadata>
    {
        protected override int GetMetadataLengthAbs(MessageMetadata metadata) => 4;

        protected override bool TryParseMetadata(ReadOnlySequence<byte> input, out MessageMetadata metadata)
        {
            metadata = default;
            var sequenceReader = new SequenceReader(input);
            if (!sequenceReader.TryRead(BinaryPrimitives.ReadInt32LittleEndian, out var length))
                return false;

            metadata = new MessageMetadata { Length = length };
            return true;
        }

        protected override void WriteMetadataAbs(Span<byte> buffer, MessageMetadata metadata)
            => BinaryPrimitives.WriteInt32LittleEndian(buffer, metadata.Length);
    }
}
