using BotMaster.PluginSystem.Messages;
using dotVariant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NonSucking.Framework.Serialization;
using System.IO;
using BotMaster.PluginSystem;

namespace BotMaster.MessageContract
{
    [Variant]
    public partial class DefinedMessage 
    {
        static partial void VariantOf(TextMessage textMessage);

        public string TargetId { get; }

        public DefinedMessage(TextMessage textMessage, string targetId) : this(textMessage)
        {
            TargetId = targetId;
        }

        public Message ToMessage()
        {
            using var memory = new MemoryStream();
            using var writer = new BinaryWriter(memory);
            return Visit(
                        textMessage =>
                        {
                            textMessage.Serialize(writer);
                            return new Message(Contract.Id, MessageType.Defined, memory.ToArray(), TargetId);
                        }
            );
        }

        public static DefinedMessage CreateTextMessage(string text)
            => new TextMessage(text);

        public static DefinedMessage FromMessage(Message message)
        {
            if (message.Type != MessageType.Defined)
                throw new NotSupportedException("Custom messages are not supported by DefinedMessage");

            var data = message.DataAsSpan();
            var id = BitConverter.ToInt32(data[..1]);

            using var memory = new MemoryStream(data.ToArray());
            using var binaryReader = new BinaryReader(memory);

            return id switch
            {
                TextMessage.TypeId => new(TextMessage.Deserialize(binaryReader), message.TargetId),
                _ => throw new NotSupportedException($"message {id} is a unknown message type in DefinedMessage"),
            };
        }
    }

    [Nooson]
    public partial class TextMessage
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 0;

        public string Text { get; set; }

        public TextMessage(string text)
        {
            Text = text;
        }

        private TextMessage(string text, int typeId)
        {
            Text = text;

        }
    }
}
