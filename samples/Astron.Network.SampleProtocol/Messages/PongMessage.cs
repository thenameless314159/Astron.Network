namespace Astron.Network.SampleProtocol.Messages
{
    public class PongMessage
    {
        public bool Quiet { get; set; }

        public PongMessage()
        {
        }

        public PongMessage(bool quiet) => Quiet = quiet;
    }
}
