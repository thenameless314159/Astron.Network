namespace Astron.Network.Framing
{
    /// <summary>
    /// Concrete implementation of the <see cref="IMessageMetadata"/> interface.
    /// It must contains data about some message.
    /// </summary>
    public class MessageMetadata : IMessageMetadata
    {
        /// <summary>
        /// The length of the parsed message.
        /// </summary>
        public int Length { get; set; }
    }
}
