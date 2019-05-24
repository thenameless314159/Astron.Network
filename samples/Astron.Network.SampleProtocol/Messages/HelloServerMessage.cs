namespace Astron.Network.SampleProtocol.Messages
{
    public class HelloServerMessage
    {
        public string Message { get; set; }

        public HelloServerMessage()
        {
        }

        public HelloServerMessage(string message) => Message = message;
    }
}
