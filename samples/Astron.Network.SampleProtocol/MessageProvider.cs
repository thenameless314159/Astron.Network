using System;
using System.Collections.Generic;

using Astron.Network.SampleProtocol.Messages;

namespace Astron.Network.SampleProtocol
{
    public static class MessageProvider
    {
        private static Dictionary<int, Func<object>> _messages = new Dictionary<int, Func<object>>
        {
            { 1, () => new PingMessage() },
            { 2, () => new PongMessage() },
            { 3, () => new HandshakeMessage() },
            { 4, () => new HelloServerMessage() }
        };

        private static Dictionary<Type, short> _messagesType = new Dictionary<Type, short>
        {
            { typeof(PingMessage), 1  },
            { typeof(PongMessage), 2 },
            { typeof(HandshakeMessage), 3 },
            { typeof(HelloServerMessage), 4 }
        };

        public static short GetId<TMessage>() => _messagesType[typeof(TMessage)];
        public static object GetMessage(int fromId) => _messages[fromId]();

        public static bool ContainsMessage(int withId) => _messages.ContainsKey(withId);
        public static bool ContainsMessage<T>() => _messagesType.ContainsKey(typeof(T));
    }
}
