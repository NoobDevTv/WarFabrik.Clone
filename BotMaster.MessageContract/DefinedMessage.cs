using BotMaster.PluginSystem.Messages;
using NonSucking.Framework.Extension.Rx.SumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.MessageContract
{
    public class DefinedMessage : Variant<DefinedMessage.TextMessage, int>
    {
        public string TargetId { get; }

        public DefinedMessage(TextMessage textMessage, string targetId = null) : base(textMessage)
        {
            TargetId = targetId;
        }

        public record TextMessage(string Text)
        {
            public const int TypeId = 0;

            public byte[] ToArray()
            {
                var dataSize = Encoding.UTF8.GetByteCount(Text) + sizeof(int);
                var data = new byte[dataSize];

                BitConverter.TryWriteBytes(data, TypeId);
                Encoding.UTF8.GetBytes(Text, data);
                return data;
            }

            public static TextMessage FromSpan(ReadOnlySpan<byte> data)
            {
                var text = Encoding.UTF8.GetString(data);
                return new(text);
            }
        }

        public Message ToMessage()
            => Map(
                    textMessage => new Message(Contract.Id, MessageType.Defined, textMessage.ToArray(), TargetId),
                    number => new Message(Contract.Id, MessageType.Defined, null)
               );


        public static DefinedMessage CreateTextMessage(string text)
            => new TextMessage(text);

        public static DefinedMessage FromMessage(Message message)
        {
            if (message.Type != MessageType.Defined)
                throw new NotSupportedException("Custom messages are not supported by DefinedMessage");

            var data = message.DataAsSpan();
            var id = BitConverter.ToInt32(data[..1]);

            switch (id)
            {
                case TextMessage.TypeId:
                    return new(TextMessage.FromSpan(data[sizeof(int)..]), message.TargetId);
                default:
                    throw new NotSupportedException($"message {id} is a unknown message type in DefinedMessage");
            }
        }

        public static implicit operator DefinedMessage(TextMessage obj) 
            => new(obj);
    }
}
