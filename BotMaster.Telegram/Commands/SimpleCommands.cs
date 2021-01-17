using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobDevBot.Telegram.Commands
{
    public static class SimpleCommands
    {
        [Command("start")]
        public static bool Start(TelegramCommandArgs args)
        {
            DatabaseManager.InsertUserIfNotExist(args.Message.From, args.Message);
            args.Bot.SendTextMessageAsync(args.Message.Chat.Id, "Wilkommen! Weitere Hilfe kommt mit der nächsten Version");

            return true;
        }        
    }
}
