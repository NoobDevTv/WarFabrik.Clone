
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotMaster.Telegram
{
    public class TelegramCommandArgs
    {
        public Message Message { get; set; }
        public TelegramBotClient Bot { get; set; }

        public TelegramCommandArgs(Message message, TelegramBotClient bot)
        {
            Message = message;
            Bot = bot;
        }

    }
}