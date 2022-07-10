
using BotMaster.PluginSystem.Messages;

using dotVariant;

namespace BotMaster.Livestream.MessageContract
{
    [Variant]
    public partial class LivestreamMessage
    {
        static partial void VariantOf(FollowInformation followInformation, RaidInformation raidInformation, StreamLiveInformation streamLiveInformation);

        public string TargetId { get; }

        public LivestreamMessage(FollowInformation followInformation, string targetId) : this(followInformation)
        {
            TargetId = targetId;
        }
        public LivestreamMessage(RaidInformation raidInformation, string targetId) : this(raidInformation)
        {
            TargetId = targetId;
        }
        public LivestreamMessage(StreamLiveInformation streamLiveInformation, string targetId) : this(streamLiveInformation)
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
                            return new Message(LivestreamContract.UID, MessageType.Custom, memory.ToArray(), TargetId);
                        },
                        raid =>
                        {
                            raid.Serialize(writer);
                            return new Message(LivestreamContract.UID, MessageType.Custom, memory.ToArray(), TargetId);
                        },
                        liveInfo =>
                        {
                            liveInfo.Serialize(writer);
                            return new Message(LivestreamContract.UID, MessageType.Custom, memory.ToArray(), TargetId);
                        }
            );
        }

        public static LivestreamMessage FromMessage(Message message)
        {
            if (message.Type != MessageType.Custom)
                throw new NotSupportedException($"Non custom messages are not supported by {nameof(LivestreamMessage)}");

            var data = message.DataAsSpan();
            var id = BitConverter.ToInt32(data);

            using var memory = new MemoryStream(data.ToArray());
            using var binaryReader = new BinaryReader(memory);

            return id switch
            {
                FollowInformation.TypeId => new(FollowInformation.Deserialize(binaryReader), message.TargetId),
                RaidInformation.TypeId => new(RaidInformation.Deserialize(binaryReader), message.TargetId),
                StreamLiveInformation.TypeId => new(StreamLiveInformation.Deserialize(binaryReader), message.TargetId),
                _ => throw new NotSupportedException($"message {id} is a unknown message type in {nameof(LivestreamMessage)}"),
            };
        }

    }
}
