using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Astron.Network.Framing;
using Astron.Network.SampleProtocol;
using Astron.Network.SampleProtocol.Framing;
using Astron.Network.SampleProtocol.Messages;

using Pipelines.Sockets.Unofficial;

namespace Astron.Network.SampleClient
{
    public class RemoteNetworkConnection : SocketConnection<NetworkMetadata>
    {
        private static readonly FrameParser<NetworkMetadata> _parser = new NetworkFrameParser();

        public static async Task<RemoteNetworkConnection> Connect()
        {
            var socketConn = await SocketConnection.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 8080));

            var connection = new RemoteNetworkConnection
            {
                Parser = _parser,
                Pipe = socketConn
            };

            return connection;
        }

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
                case HandshakeMessage msg:
                    {
                        var reader = new BinaryReader(
                            new MemoryStream(frame.Payload.ToArray())); // forced to reallocate :(
                        MessageSerializer.Deserialize(msg, reader);
                        Console.WriteLine($"Received HandshakeMessage with protocolVersion={msg.ProtocolVersion}, attempting to send HelloServerMessage with message='Greetings from andromeda !'");
                        await SendMessageAsync(new HelloServerMessage("Greetings from andromeda !"));
                        break;
                    }
            }
        }

        public async ValueTask<FlushResult> SendMessageAsync<T>(T message)
        {
            if (!MessageProvider.ContainsMessage<T>()) throw new ArgumentException(nameof(message));
            if (message == null) throw new ArgumentNullException(nameof(message));

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
