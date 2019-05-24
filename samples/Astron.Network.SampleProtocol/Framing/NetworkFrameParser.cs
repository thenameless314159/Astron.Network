using System;
using System.Buffers;
using System.Buffers.Binary;

using Astron.Network.Buffers;
using Astron.Network.Framing;

namespace Astron.Network.SampleProtocol.Framing
{
    public class NetworkFrameParser : FrameParser<NetworkMetadata>
    {
        private const int _metadataLength = sizeof(short) + sizeof(int);

        protected override int GetMetadataLengthAbs(NetworkMetadata metadata) => _metadataLength;

        protected override bool TryParseMetadata(ReadOnlySequence<byte> input, out NetworkMetadata metadata)
        {
            metadata = default;
            var sequenceReader = new SequenceReader(input);
            if (!sequenceReader.TryRead(BinaryPrimitives.ReadInt16LittleEndian, out var messageId))
                return false;

            if (!sequenceReader.TryRead(BinaryPrimitives.ReadInt32LittleEndian, out var length))
                return false;

            metadata = new NetworkMetadata { MessageId = messageId, Length = length };
            return true;
        }

        protected override void WriteMetadataAbs(Span<byte> buffer, NetworkMetadata metadata)
        {
            BinaryPrimitives.WriteInt16LittleEndian(buffer, metadata.MessageId);
            BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(2), metadata.Length);
        }
    }
}
