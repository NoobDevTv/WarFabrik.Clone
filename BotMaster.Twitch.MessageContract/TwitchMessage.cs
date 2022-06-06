using BotMaster.PluginSystem.Messages;

using dotVariant;

namespace BotMaster.Twitch.MessageContract
{
    [Variant]
    public partial class TwitchMessage
    {
        static partial void VariantOf(FollowInformation followInformation, RaidInformation raidInformation);

        public string TargetId { get; }

        public TwitchMessage(FollowInformation followInformation, string targetId) : this(followInformation)
        {
            TargetId = targetId;
        }
        public TwitchMessage(RaidInformation raidInformation, string targetId) : this(raidInformation)
        {
            TargetId = targetId;
        }

        public Message ToMessage()
        {
            using var memory = new MemoryStream();
            using var writer = new BinaryWriter(memory);
            return Visit(
                        follower =>
                        {
                            follower.Serialize(writer);
                            return new Message(Contract.UID, MessageType.Custom, memory.ToArray(), TargetId);
                        },
                        raid =>
                        {
                            raid.Serialize(writer);
                            return new Message(Contract.UID, MessageType.Custom, memory.ToArray(), TargetId);
                        }
            );
        }

        public static TwitchMessage FromMessage(Message message)
        {
            if (message.Type != MessageType.Custom)
                throw new NotSupportedException($"Non custom messages are not supported by {nameof(TwitchMessage)}");

            var data = message.DataAsSpan();
            var id = BitConverter.ToInt32(data);

            using var memory = new MemoryStream(data.ToArray());
            using var binaryReader = new BinaryReader(memory);

            return id switch
            {
                FollowInformation.TypeId => new(FollowInformation.Deserialize(binaryReader), message.TargetId),
                RaidInformation.TypeId => new(RaidInformation.Deserialize(binaryReader), message.TargetId),
                _ => throw new NotSupportedException($"message {id} is a unknown message type in {nameof(TwitchMessage)}"),
            };
        }

    }
}
