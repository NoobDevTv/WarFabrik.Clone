using BotMaster.PluginSystem.Messages;

using dotVariant;

namespace BotMaster.MessageContract
{
    [Variant]
    public partial class DefinedMessage
    {
        static partial void VariantOf(TextMessage textMessage, CommandMessage commandMessage);

        public string TargetId { get; }

        public DefinedMessage(TextMessage textMessage, string targetId) : this(textMessage)
        {
            TargetId = targetId;
        }
        public DefinedMessage(CommandMessage commandMessage, string targetId) : this(commandMessage)
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
                        },
                        commandMessage =>
                        {
                            commandMessage.Serialize(writer);
                            return new Message(Contract.Id, MessageType.Defined, memory.ToArray(), TargetId);
                        }
            );
        }

        public static DefinedMessage CreateTextMessage(string text)
            => new TextMessage(text);

        public static DefinedMessage CreateCommandMessage(string command, IReadOnlyCollection<string> parameter)
            => new CommandMessage(command, parameter);

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
                CommandMessage.TypeId => new(CommandMessage.Deserialize(binaryReader), message.TargetId),
                _ => throw new NotSupportedException($"message {id} is a unknown message type in DefinedMessage"),
            };
        }
    }
}
