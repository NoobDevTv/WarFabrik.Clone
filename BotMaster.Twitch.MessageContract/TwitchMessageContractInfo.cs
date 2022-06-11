﻿using BotMaster.PluginSystem;

namespace BotMaster.Twitch.MessageContract
{
    public readonly record struct TwitchMessageContractInfo(Guid UID, string Name) : IMessageContractInfo
    {
        public static TwitchMessageContractInfo Create()
            => new(
                TwitchContract.UID,
                nameof(TwitchMessageContractInfo));
    }
}
