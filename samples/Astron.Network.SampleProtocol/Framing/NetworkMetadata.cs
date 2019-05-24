using Astron.Network.Framing;

namespace Astron.Network.SampleProtocol.Framing
{
    public class NetworkMetadata : IMessageMetadata
    {
        public int Length { get; set; }
        public short MessageId { get; set; }

        public NetworkMetadata()
        {
        }

        public NetworkMetadata(int length, short messageId)
        {
            Length = length;
            MessageId = messageId;
        }
    }
}
