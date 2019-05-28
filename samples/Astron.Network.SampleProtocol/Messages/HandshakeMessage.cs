namespace Astron.Network.SampleProtocol.Messages
{
    public class HandshakeMessage
    {
        public string ProtocolVersion { get; set; }

        public HandshakeMessage()
        {
        }

        public HandshakeMessage(string protocolVersion) => ProtocolVersion = protocolVersion;
    }
}
