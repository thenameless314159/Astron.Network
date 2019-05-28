using System;
using System.Buffers;

namespace Astron.Network.Framing
{
    /// <summary>
    /// Base class containing the frame parsing logic.
    /// </summary>
    public abstract class FrameParser
    {
        public abstract void WriteMetadata(Span<byte> buffer, IMessageMetadata metadata);
        public abstract int GetMetadataLength(IMessageMetadata metadata);
        public abstract bool TryParseFrame(ref ReadOnlySequence<byte> buffer, out Frame frame);
        public int GetFrameLengthFrom(IMessageMetadata metadata) => GetMetadataLength(metadata) + metadata.Length;
        public int GetFrameLength(Frame frame) => GetFrameLengthFrom(frame.Meta);
    }

    /// <summary>
    /// Base class containing the frame parsing logic.
    /// You must provide a concrete implementation of <see cref="IMessageMetadata"/>.
    /// </summary>
    /// <typeparam name="T">A concrete implementation of <see cref="IMessageMetadata"/>.</typeparam>
    public abstract class FrameParser<T> : FrameParser
        where T : IMessageMetadata
    {
        public override void WriteMetadata(Span<byte> buffer, IMessageMetadata metadata) => WriteMetadataAbs(buffer, (T)metadata);
        public override int GetMetadataLength(IMessageMetadata meta) => GetMetadataLengthAbs((T)meta);

        protected abstract int GetMetadataLengthAbs(T metadata);
        protected abstract void WriteMetadataAbs(Span<byte> buffer, T metadata);

        protected abstract bool TryParseMetadata(ReadOnlySequence<byte> input, out T metadata);

        public override bool TryParseFrame(ref ReadOnlySequence<byte> buffer, out Frame frame)
        {
            if (!_tryParseFrame(ref buffer, out var tmp))
            {
                frame = default;
                return false;
            }

            frame = tmp;
            return true;
        }

        private bool _tryParseFrame(ref ReadOnlySequence<byte> buffer, out Frame<T> frame)
        {
            frame = default;
            if (!TryParseMetadata(buffer, out var metadata))
                return false;

            if (buffer.Length < GetFrameLengthFrom(metadata)) //not enough data to read whole frame
                return false;

            var payload = buffer.Slice(GetMetadataLength(metadata), metadata.Length);
            buffer = buffer.Slice(payload.End);

            frame = new Frame<T>(payload, metadata);
            return true;
        }
    }
}
