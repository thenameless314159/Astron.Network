using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Astron.Network.Framing
{
    public class Frame
    {
        public Memory<byte>     Payload { get; }
        public IMessageMetadata Meta    { get; }

        public Frame(ReadOnlySequence<byte> payload, IMessageMetadata metadata)
        {
            Meta    = metadata;
            Payload = payload.ToArray();
        }
    }

    public class Frame<T> : Frame where T : IMessageMetadata
    {
        public T Metadata => (T)Meta;

        public Frame(ReadOnlySequence<byte> payload, T meta) : base(payload, meta)
        {
        }
    }
}
