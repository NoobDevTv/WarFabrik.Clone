using BotMaster.PluginSystem.Messages;

using dotVariant;

namespace BotMaster.Betterplace.MessageContract
{
    [Variant]
    public partial class BetterplaceMessage
    {
        static partial void VariantOf(Donation donation);

        public string TargetId { get; }

        public BetterplaceMessage(Donation donation, string targetId) : this(donation)
        {
            TargetId = targetId;
        }

        public Message ToMessage()
        {
            using var memory = new MemoryStream();
            using var writer = new BinaryWriter(memory);
            return Visit(
                        donation =>
                        {
                            donation.Serialize(writer);
                            return new Message(Contract.Id, MessageType.Defined, memory.ToArray(), TargetId);
                        }
            );
        }


        public static BetterplaceMessage FromMessage(Message message)
        {
            if (message.Type != MessageType.Defined)
                throw new NotSupportedException("Custom messages are not supported by DefinedMessage");

            var data = message.DataAsSpan();
            var id = BitConverter.ToInt32(data[..1]);

            using var memory = new MemoryStream(data.ToArray());
            using var binaryReader = new BinaryReader(memory);

            return id switch
            {
                Donation.TypeId => new(Donation.Deserialize(binaryReader), message.TargetId),
                _ => throw new NotSupportedException($"message {id} is a unknown message type in DefinedMessage"),
            };
        }

    }
}
