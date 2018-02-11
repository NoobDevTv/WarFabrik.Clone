using TwitchLib;
using TwitchLib.Models.Client;

namespace WarFabrik.Clone
{
    public class BotCommandArgs
    {
        public Bot Bot { get; private set; }
        public TwitchAPI TwitchAPI { get; private set; }
        public ChatMessage Message { get; private set; }

        public BotCommandArgs(Bot bot, TwitchAPI api,ChatMessage message)
        {
            Bot = bot;
            Message = message;
            TwitchAPI = api;
        }
    }
}