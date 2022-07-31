using BotMaster.PluginSystem;

namespace BotMaster.MessageContract
{
    public readonly record struct DefaultMessageContractInformation(Guid UID, string Name) : IMessageContractInfo
    {
        public static DefaultMessageContractInformation Create()
            => new(
                new Guid("A4B5EF4B-248B-4831-A6F4-6EB1ED6088D2"), 
                nameof(DefaultMessageContractInformation)
            );
    }
}
