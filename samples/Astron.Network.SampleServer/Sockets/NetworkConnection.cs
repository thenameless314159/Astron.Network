using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

using Astron.Network.Framing;
using Astron.Network.SampleProtocol;
using Astron.Network.SampleProtocol.Framing;
using Astron.Network.SampleProtocol.Messages;

namespace Astron.Network.SampleServer.Sockets
{
    public class NetworkConnection : SocketConnection<NetworkMetadata>
    {
        protected override async ValueTask OnReceiveAsync(Frame<NetworkMetadata> frame)
        {
            var metadata = frame.Metadata;
            if (!MessageProvider.ContainsMessage(metadata.MessageId))
            {
                Console.WriteLine($"Unknown message received with id : {metadata.MessageId}, disconnecting...");
                Dispose();
            }

            var message = MessageProvider.GetMessage(metadata.MessageId);
            switch (message)
            {
                case PingMessage msg:
                {
                     var reader = new BinaryReader(
                         new MemoryStream(frame.Payload.ToArray())); // forced to reallocate :(
                     MessageSerializer.Deserialize(msg, reader);
                     Console.WriteLine($"Received PingMessage with quiet={msg.Quiet}, attempting to send PongMessage...");
                     await SendMessageAsync(new PongMessage(msg.Quiet));
                     break;
                }
                case PongMessage msg:
                {
                    var reader = new BinaryReader(
                        new MemoryStream(frame.Payload.ToArray())); // forced to reallocate :(
                    MessageSerializer.Deserialize(msg, reader);
                    Console.WriteLine($"Received PongMessage with quiet={msg.Quiet}, attempting to send HandshakeMessage with protocolVersion=1.0.0");
                    await SendMessageAsync(new HandshakeMessage("1.0.0"));
                    break;
                }
                case HelloServerMessage msg:
                {
                    var reader = new BinaryReader(
                        new MemoryStream(frame.Payload.ToArray())); // forced to reallocate :(
                    MessageSerializer.Deserialize(msg, reader);
                    Console.WriteLine($"Received HelloServerMessage with message='{msg.Message}', work successfully completed, closing connection...");
                    Dispose();
                    break;
                }
            }
        }

        public async ValueTask<FlushResult> SendMessageAsync<T>(T message)
        {
            if(!MessageProvider.ContainsMessage<T>()) throw new ArgumentException(nameof(message));
            if(message == null) throw new ArgumentNullException(nameof(message));

            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            MessageSerializer.Serialize(message, writer);

            var payload = stream.GetBuffer();
            var result = await SendAsync(new Frame<NetworkMetadata>(payload,
                new NetworkMetadata(payload.Length, MessageProvider.GetId<T>())));

            Console.WriteLine($"{typeof(T).Name} successfully sent !");
            return result;
        }

        protected override ValueTask OnDestroyAsync()
        {
            Console.WriteLine("Client disconnected");
            return base.OnDestroyAsync();
        }
    }
}
