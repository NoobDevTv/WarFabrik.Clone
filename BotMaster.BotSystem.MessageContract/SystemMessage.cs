using BotMaster.PluginSystem.Messages;

using dotVariant;

namespace BotMaster.BotSystem.MessageContract
{
    [Variant]
    public partial class SystemMessage
    {
        static partial void VariantOf(GetPlugins getPlugin, PluginList pluginList, PluginCommand pluginCommand, PluginInfo pluginInfo, CommandsChanged commandChanged);

        public string TargetId { get; init; }

        public Message ToMessage()
        {
            using var memory = new MemoryStream();
            using var writer = new BinaryWriter(memory);
            Visit(
                getPlugin => getPlugin.Serialize(writer),
                pluginList => pluginList.Serialize(writer),
                pluginCommand => pluginCommand.Serialize(writer),
                pluginInfo => pluginInfo.Serialize(writer),
                commandChanged => commandChanged.Serialize(writer)
            );
            return new Message(SystemContract.UID, MessageType.Custom, memory.ToArray(), TargetId);
        }


        public static SystemMessage FromMessage(Message message)
        {
            if (message.Type != MessageType.Custom)
                throw new NotSupportedException($"Non custom messages are not supported by {nameof(SystemMessage)}");

            var data = message.DataAsSpan();
            var id = BitConverter.ToInt32(data);

            using var memory = new MemoryStream(data.ToArray());
            using var binaryReader = new BinaryReader(memory);

            return id switch
            {
                GetPlugins.TypeId => new(GetPlugins.Deserialize(binaryReader)) { TargetId = message.TargetId },
                PluginList.TypeId => new(PluginList.Deserialize(binaryReader)) { TargetId = message.TargetId },
                PluginCommand.TypeId => new(PluginCommand.Deserialize(binaryReader)) { TargetId = message.TargetId },
                PluginInfo.TypeId => new(PluginInfo.Deserialize(binaryReader)) { TargetId = message.TargetId },
                CommandsChanged.TypeId => new(CommandsChanged.Deserialize(binaryReader)) { TargetId = message.TargetId },
                _ => throw new NotSupportedException($"message {id} is a unknown message type in {nameof(SystemMessage)}"),
            };
        }

    }
}
