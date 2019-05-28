using System;
using System.IO;

using Astron.Network.SampleProtocol.Messages;

namespace Astron.Network.SampleProtocol
{
    public static class MessageSerializer
    {
        static MessageSerializer()
        {
            Serializer<PingMessage>.Serialize = (bw, msg) => { bw.Write(msg.Quiet); };
            Serializer<PongMessage>.Serialize = (bw, msg) => { bw.Write(msg.Quiet); };
            Serializer<HandshakeMessage>.Serialize = (bw, msg) => { bw.Write(msg.ProtocolVersion); };
            Serializer<HelloServerMessage>.Serialize = (bw, msg) => { bw.Write(msg.Message); };

            Deserializer<PingMessage>.Deserialize = (br, msg) => { msg.Quiet = br.ReadBoolean(); };
            Deserializer<PongMessage>.Deserialize = (br, msg) => { msg.Quiet = br.ReadBoolean(); };
            Deserializer<HandshakeMessage>.Deserialize = (br, msg) => { msg.ProtocolVersion = br.ReadString(); };
            Deserializer<HelloServerMessage>.Deserialize = (br, msg) => { msg.Message = br.ReadString(); };
        }

        public static void Serialize<T>(T message, BinaryWriter writer)
            => Serializer<T>.Serialize(writer, message);

        public static void Deserialize<T>(T message, BinaryReader reader)
            => Deserializer<T>.Deserialize(reader, message);

        static class Serializer<T>
        {
            public static Action<BinaryWriter, T> Serialize;
        }

        static class Deserializer<T>
        {
            public static Action<BinaryReader, T> Deserialize;
        }
    }
}
