using System;
using System.Collections.Generic;
using System.Text;

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
