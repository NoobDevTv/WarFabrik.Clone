using BotMaster.PluginSystem;

namespace BotMaster.Livestream.MessageContract
{
    public readonly record struct LivestreamMessageContractInfo(Guid UID, string Name) : IMessageContractInfo
    {
        public static LivestreamMessageContractInfo Create()
            => new(
                LivestreamContract.UID,
                nameof(LivestreamMessageContractInfo));
    }
}
