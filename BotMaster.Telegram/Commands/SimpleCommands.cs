
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
                context.Client.SendTextMessageAsync(new ChatId(long.Parse(message.PlattformUserId)), $"Enter your connection code into the application you want to connect to with the syntax \"connect 'code'\". It's valid for one hour: \"{UserConnectionService.StartConnection(message.PlattformUserId)}\"");
            }
        }

        static string[] subscriptionsNames = new string[] { "livestream", "text", "chat", "donation", "raid", "follower" };
        internal static bool Subscribe(TelegramContext context, CommandMessage message)
        {
            using var ctx = new RightsDbContext();
            var plattformUser = ctx.PlattformUsers.FirstOrDefault(x => x.Name == message.Username && x.Platform == message.SourcePlattform);

            if (plattformUser is null)
            {
                context.Client.SendTextMessageAsync(new(long.Parse(message.PlattformUserId)), "You didn't connect first with /start");
                return false;
            }

            var subscription = message.Parameter.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(subscription))
            {
                context.Client.SendTextMessageAsync(new(long.Parse(message.PlattformUserId)), $"You have to pass a parameter what you want to subscribe. Possible values are: {string.Join(",", subscriptionsNames)}");
                return false;
            }
            subscription = subscription.ToLower();
            if (!subscriptionsNames.Contains(subscription))
            {
                context.Client.SendTextMessageAsync(new(long.Parse(message.PlattformUserId)), $"You have to pass a parameter known parameter. Possible values are: {string.Join(",", subscriptionsNames)}");
                return false;
            }

            var groupToJoin = ctx.Groups.FirstOrDefault(x => x.Name == subscription);
            if (groupToJoin is null)
            {
                groupToJoin = new() { Name = subscription, PlattformUsers = new List<PlattformUser> { plattformUser }, IsDefault = false };
                ctx.Groups.Add(groupToJoin);
            }

            if (!groupToJoin.PlattformUsers.Contains(plattformUser))
                groupToJoin.PlattformUsers.Add(plattformUser);



            ctx.SaveChanges();

            context.Client.SendTextMessageAsync(new(long.Parse(message.PlattformUserId)), $"You have sucessfully subscribed to {subscription}");
            return true;
        }


        internal static bool Unsubscribe(TelegramContext context, CommandMessage message)
        {
            using var ctx = new RightsDbContext();
            var plattformUser = ctx.PlattformUsers.FirstOrDefault(x => x.Name == message.Username && x.Platform == message.SourcePlattform);

            if (plattformUser is null)
            {
                context.Client.SendTextMessageAsync(new(long.Parse(message.PlattformUserId)), "You didn't connect first with /start");
                return false;
            }

            var groupNames = plattformUser.Groups.Select(x => x.Name).ToArray();


            var subscription = message.Parameter.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(subscription))
            {
                context.Client.SendTextMessageAsync(new(long.Parse(message.PlattformUserId)), $"You have to pass a parameter what you want to unsubscribe. Possible values are: {string.Join(",", groupNames)}");
                return false;
            }

            subscription = subscription.ToLower();
            if (!groupNames.Contains(subscription))
            {
                context.Client.SendTextMessageAsync(new(long.Parse(message.PlattformUserId)), $"You have to pass a parameter known parameter. Possible values are: {string.Join(",", groupNames)}");
                return false;
            }


            var groupToLeave = ctx.Groups.FirstOrDefault(x => x.Name == subscription);
            if (groupToLeave is null)
            {
                context.Client.SendTextMessageAsync(new(long.Parse(message.PlattformUserId)), $"Internal server errror occured, please contact your administrator and try again");
                return false; //Should not be possible
            }

            groupToLeave.PlattformUsers.Remove(plattformUser);

            ctx.SaveChanges();

            context.Client.SendTextMessageAsync(new(long.Parse(message.PlattformUserId)), $"You have sucessfully unsubscribed to {subscription}");
            return true;
        }
    }
}
