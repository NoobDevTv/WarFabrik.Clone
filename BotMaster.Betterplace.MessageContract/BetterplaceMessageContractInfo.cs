using BotMaster.PluginSystem;
using System;

namespace BotMaster.Betterplace.MessageContract
{
    public readonly record struct BetterplaceMessageContractInfo(Guid UID, string Name) : IMessageContractInfo
    {
        public static BetterplaceMessageContractInfo Create()
            => new(
                new Guid("524FED8B-38C6-4241-B5A0-84752A6964AD"),
                nameof(BetterplaceMessageContractInfo));
    }
}
