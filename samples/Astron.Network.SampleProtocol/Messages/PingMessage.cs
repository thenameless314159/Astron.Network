namespace Astron.Network.SampleProtocol.Messages
{
    public class PingMessage
    {
        public bool Quiet { get; set; }

        public PingMessage()
        {
        }

        public PingMessage(bool quiet) => Quiet = quiet;
    }
}
