
using BotMaster.MessageContract;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Database;

using CommandManagementSystem.Attributes;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotMaster.Telegram.Commands
{
    internal static class SimpleCommands
    {
        internal static void Start(CommandMessage args, TelegramContext botContext)
        {
            if (!string.Equals(args.SourcePlattform, "telegram", StringComparison.InvariantCultureIgnoreCase))
                return;

            using var context = new TelegramDBContext();
            var plattformUser = context.PlatformUsers.FirstOrDefault(x => x.Name == args.Username && string.Equals(x.Platform, args.SourcePlattform, StringComparison.InvariantCultureIgnoreCase));
            if (plattformUser is not null)
            {
                botContext.Client.SendTextMessageAsync(new ChatId(long.Parse(plattformUser.PlattformUserId)), "You were already registered.");
                return;
            }

            context.PlatformUsers.Add(new PlattformUser() { Name = args.Username, Platform = args.SourcePlattform, PlattformUserId = args.PlattformUserId });
            context.SaveChanges();
            botContext.Client.SendTextMessageAsync(new ChatId(long.Parse(plattformUser.PlattformUserId)), "Welcome! No additional help will be implemented in the next update");

        }
    }
}
