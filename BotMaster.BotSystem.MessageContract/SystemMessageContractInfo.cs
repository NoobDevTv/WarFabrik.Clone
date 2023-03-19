using BotMaster.PluginSystem;

namespace BotMaster.BotSystem.MessageContract
{
    public readonly record struct SystemMessageContractInfo(Guid UID, string Name) : IMessageContractInfo
    {
        public static SystemMessageContractInfo Create()
            => new(
                SystemContract.UID,
                nameof(SystemMessageContractInfo));
    }
}
