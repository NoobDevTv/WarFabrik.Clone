
using BotMaster.Commandos;
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

            using var context = new RightsDbContext();
            var plattformUser = context.PlattformUsers.FirstOrDefault(x => x.Name == args.Username && x.Platform == args.SourcePlattform);
            if (plattformUser is not null)
            {
                botContext.Client.SendTextMessageAsync(new ChatId(long.Parse(plattformUser.PlattformUserId)), "You were already registered.");
                return;
            }
            plattformUser = new PlattformUser() { Name = args.Username, Platform = args.SourcePlattform, PlattformUserId = args.PlattformUserId };
            context.PlattformUsers.Add(plattformUser);
            context.SaveChanges();
            botContext.Client.SendTextMessageAsync(new ChatId(long.Parse(plattformUser.PlattformUserId)), "Welcome! No additional help will be implemented in the next update");

        }

        internal static void SendTextCommand(CommandMessage commandMessage, PersistentCommand command, TelegramContext botContext)
        {
            if (command.Secure && !commandMessage.Secure)
                return;

            botContext.Client.SendTextMessageAsync(new ChatId(long.Parse(commandMessage.PlattformUserId)), command.Text);
        }
    }
}
