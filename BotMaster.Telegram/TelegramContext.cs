﻿using BotMaster.Commandos;

using Telegram.Bot;

namespace BotMaster.Telegram
{

    internal record TelegramContext(TelegramBotClient Client, CommandoCentral CommandoCentral) : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
