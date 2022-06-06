using BotMaster.PluginSystem;

namespace BotMaster.Betterplace.MessageContract
{
    public readonly record struct BetterplaceMessageContractInfo(Guid UID, string Name) : IMessageContractInfo
    {
        public static BetterplaceMessageContractInfo Create()
            => new(
                Contract.UID,
                nameof(BetterplaceMessageContractInfo));
    }
}
