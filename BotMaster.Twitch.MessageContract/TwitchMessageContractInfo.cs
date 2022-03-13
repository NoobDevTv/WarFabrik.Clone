using BotMaster.PluginSystem;

using System;

namespace BotMaster.Betterplace.MessageContract
{
    public readonly record struct TwitchMessageContractInfo(Guid UID, string Name) : IMessageContractInfo
    {
        public static TwitchMessageContractInfo Create()
            => new(
                Contract.UID,
                nameof(TwitchMessageContractInfo));
    }
}
