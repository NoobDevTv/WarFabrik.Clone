
using BotMaster.Commandos;
using BotMaster.MessageContract;
using BotMaster.PluginSystem.Messages;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Database;

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

        internal static void Connect(TelegramContext context, CommandMessage message)
        {
            if (message.Parameter.Count > 0)
{
                if (UserConnectionService.EndConnection(message.PlattformUserId, message.Parameter.First()))
                    context.Client.SendTextMessageAsync(new ChatId(long.Parse(message.PlattformUserId)), $"You have connected successfully");
                else
                    context.Client.SendTextMessageAsync(new ChatId(long.Parse(message.PlattformUserId)), $"You have connected unsuccessfully, did you try to connect to the same plattform or did you already link these plattforms?");
            }
            else
            {
                context.Client.SendTextMessageAsync(new ChatId(long.Parse(message.PlattformUserId)), $"Enter your connection code into the application you want to connect to with the syntax \"connect 'code'\". It's valid for one hour: {UserConnectionService.StartConnection(message.PlattformUserId)}");
            }
        }
    }
}
