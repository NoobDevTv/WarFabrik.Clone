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

        [Command("mygroups")]
        public static bool MyGroups(TelegramCommandArgs args)
        {
            var user = DatabaseManager.GetUserById(args.Message.From.Id);

            var groups = DatabaseManager.GetGroupsFromUser(user);

            string s = "Die Gruppen denen du angehörig bist: ";
            groups.Select(x => x.Name + ", ").ToList().ForEach(x=>s+=x);
            args.Bot.SendTextMessageAsync(args.Message.Chat.Id, s.Substring(0, s.Length - 2));
            return true;
        }
    }
}
