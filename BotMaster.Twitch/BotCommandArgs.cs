
using TwitchLib.Api;
using TwitchLib.Client.Models;

namespace BotMaster.Twitch
{
    public class BotCommandArgs
    {
        public Bot Bot { get; private set; }
        public TwitchAPI TwitchAPI { get; private set; }
        public ChatMessage Message { get; private set; }

        public BotCommandArgs(Bot bot, TwitchAPI api, ChatMessage message)
        {
            Bot = bot;
            Message = message;
            TwitchAPI = api;
        }
    }
}