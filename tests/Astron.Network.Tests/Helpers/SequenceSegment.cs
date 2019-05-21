using System;
using System.Buffers;

namespace Astron.Network.Tests.Helpers
{
    public class SequenceSegment : ReadOnlySequenceSegment<byte>
    {
        public SequenceSegment(ReadOnlyMemory<byte> memory) => Memory = memory;

        public SequenceSegment Add(ReadOnlyMemory<byte> mem)
        {
            var segment = new SequenceSegment(mem)
            {
                RunningIndex = RunningIndex + Memory.Length
            };
            Next = segment;
            return segment;
        }
    }
}
