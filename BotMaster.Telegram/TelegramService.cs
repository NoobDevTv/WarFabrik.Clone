using BotMaster.Betterplace.MessageContract;
using BotMaster.Commandos;
using BotMaster.Core.NLog;
using BotMaster.Livestream.MessageContract;
using BotMaster.MessageContract;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Commands;

using Microsoft.EntityFrameworkCore;

using NLog;

using System.Reactive.Linq;

using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using DefinedMessageContract = BotMaster.MessageContract.Contract;
using MessageType = Telegram.Bot.Types.Enums.MessageType;
using PluginMessage = BotMaster.PluginSystem.Messages.Message;

namespace BotMaster.Telegram
{
    internal partial class TelegramService : Plugin
    {
        private readonly IMessageContractInfo[] messageContracts;
        private static Logger logger;

        public TelegramService()
        {
            messageContracts = new[]
            {
                (IMessageContractInfo)BetterplaceMessageContractInfo.Create()
            };
        }
        public override IEnumerable<IMessageContractInfo> ConsumeContracts() => messageContracts;

        public override IObservable<Package> Start(ILogger logger, IObservable<Package> receivedPackages)
        {
            using (var ctx = new RightsDbContext())
                ctx.Migrate();

            using (var ctx = new UserConnectionContext())
                ctx.Migrate();

            return Observable
            .Using(
                CreateBot,
                botInstance
                    => MessageConvert
                        .ToPackage(
                            Create(MessageConvert.ToMessage(receivedPackages), botInstance)
                            .OnError(logger, nameof(Create))
                        )
            );
        }

        private TelegramContext CreateBot()
        {
            var info = new FileInfo(Path.Combine(".", "additionalfiles", "Token.txt"));
            if (!info.Directory.Exists)
                info.Directory.Create();

            var token = System.IO.File.ReadAllText(info.FullName);
            var client = new TelegramBotClient(token);
            return new(client, new CommandoCentral());
        }

        private static IObservable<PluginMessage> Create(IObservable<PluginMessage> notifications, TelegramContext botContext)
        {
            logger = LogManager.GetCurrentClassLogger();
            var client = botContext.Client;

            notifications = notifications
                    .Log(logger, nameof(TelegramService) + " Incomming", onNext: LogLevel.Debug)
                    .Publish()
                    .RefCount();

            CreateIncommingCommandCallbacks(botContext);


            using var context = new RightsDbContext();

            var noobDevGroup
                = context.Groups
                    .Include(x => x.Users)
                    .ThenInclude(x => x.PlatformIdentities)
                    .FirstOrDefault(x => x.Name == "NoobDev");

            var noobDevGroupUser
                = noobDevGroup?
                .PlattformUsers
                .Concat(noobDevGroup.Users.SelectMany(u => u.PlatformIdentities))
                .Where(x => x.Platform == "Telegram")
                .Select(x => long.Parse(x.PlattformUserId))
                .Distinct()
                .ToList() ?? new List<long>();


            var definedMessages = DefinedMessageContract
                    .ToDefineMessages(notifications)
                    .Log(logger, nameof(TelegramService) + " Incomming", onNext: LogLevel.Debug)
                    .Publish()
                    .RefCount();


            var incommingLivestreamMessages = LivestreamContract
               .ToDefineMessages(notifications)
               .VisitMany(
                    follower => follower
                        .SelectMany(message =>
                        {
                            var userIds = GetIdsOfGroup("follower");
                            foreach (var item in noobDevGroupUser)
                            {
                                userIds.Add(item);
                            }

                            return userIds.Select(userId => (userId, message));

                        })
                        .Do(toSend => client.SendTextMessageAsync(new ChatId(toSend.userId), $"{toSend.message.UserName} hat soeben auf {toSend.message.SourcePlattform} gefollowed"))
                        .Select(x => (LivestreamMessage)x.message),
                    raid => raid
                        .SelectMany(message =>
                        {

                            var userIds = GetIdsOfGroup("raid");
                            foreach (var item in noobDevGroupUser)
                            {
                                userIds.Add(item);
                            }

                            return userIds.Select(userId => (userId, message));

                        })
                            .Do(toSend => client.SendTextMessageAsync(new ChatId(toSend.userId), $"{toSend.message.UserName} hat mit {toSend.message.Count} Noobs geraidet"))
                        .Select(x => (LivestreamMessage)x.message),
                    live => live
                        .Where(x => x.SourcePlattform != "Telegram")
                        .SelectMany(message =>
                        {

                            var userIds = GetIdsOfGroup("livestream");
                            foreach (var item in noobDevGroupUser)
                            {
                                userIds.Add(item);
                            }

                            return userIds.Select(userId => (userId, message));

                        })
                        .Do(toSend => client.SendTextMessageAsync(new ChatId(toSend.userId), $"Der Broadcast hat begonnen auf {toSend.message.SourcePlattform} für {toSend.message.UserName}"))
                        .Select(x => (LivestreamMessage)x.message)
                   )
                  .Log(logger, "incommingLivestreamMessages", subscription: LogLevel.Debug);

            var incommingBetterplaceMessages = BetterplaceContract
                  .ToDefineMessages(notifications)
                  .VisitMany(
                       donation => donation
                            .SelectMany(donation =>
                            {

                                var userIds = GetIdsOfGroup("donation");
                                foreach (var item in noobDevGroupUser)
                                {
                                    userIds.Add(item);
                                }

                                return userIds.Select(userId => (userId, donation));

                            })
                            .Do(toSend => client.SendTextMessageAsync(new ChatId(toSend.userId), $"{toSend.donation.Author} hat {toSend.donation.Donated_amount_in_cents} Geld gespendet. Vielen lieben Dank dafür <3"))
                            .Select(x => (BetterplaceMessage)x.donation)
                   )
                  .Log(logger, "incommingBetterplaceMessages", subscription: LogLevel.Debug);

            //Messages from other plugins
            IObservable<DefinedMessage> pluginMessageWithGroups
                = definedMessages
                .VisitMany(textMessage => textMessage
                        .SelectMany(message =>
                        {

                            var userIds = GetIdsOfGroup("text");

                            return userIds.Select(userId => (userId, message));
                        })
                        .Do(toSend => client.SendTextMessageAsync(new ChatId(toSend.userId), $"{toSend.message.Text}"))
                        .Select(x => (DefinedMessage)x.message),

                        commandMessage => commandMessage
                            .Select(x => (DefinedMessage)x),
                        chatMessage => chatMessage
                            .SelectMany(message =>
                            {
                                var userIds = GetIdsOfGroup("chat");

                                return userIds.Select(userId => (userId, message));
                            })
                            .Do(toSend => client.SendTextMessageAsync(new ChatId(toSend.userId), $"[{toSend.message.Username} ({toSend.message.Source})]: {toSend.message.Text}"))
                            .Select(x => (DefinedMessage)x.message)
                )
                .Log(logger, "pluginMessageWithGroups", subscription: LogLevel.Debug);

            var incommingCommandStream = botContext.CommandoCentral.CreateCommandStream(definedMessages
                .Match<CommandMessage>(n => n));


            //Messages from User from telegram
            IObservable<(string, TelegramCommandArgs)> chatMessages = CreateCommands(client);

            var commandMessages = chatMessages
                .Select(cm =>
                {
                    using var c = new RightsDbContext();
                    var plattformUser = c.PlattformUsers.FirstOrDefault(pu => pu.PlattformUserId == cm.Item2.Message.Chat.Id.ToString());
                    var user = plattformUser?.User;
                    return (message: cm, plattformUser, user);
                })
                .Select(x =>
                {
                    string[] split = x.message.Item2.Message.Text.TrimStart('/').Split(' ');

                    return DefinedMessage.CreateCommandMessage(split[0].ToLower(), x.message.Item2.Message.From.Username, x.user?.Id ?? -1, x.plattformUser?.PlattformUserId ?? x.message.Item2.Message.From.Id.ToString(), "Telegram", true, split[1..]);
                });
            var liveStreamMessages = chatMessages
                .Select(cm =>
                {
                    using var c = new RightsDbContext();
                    var plattformUser = c.PlattformUsers.FirstOrDefault(pu => pu.PlattformUserId == cm.Item2.Message.Chat.Id.ToString());
                    var user = plattformUser?.User;
                    return (message: cm, plattformUser, user);
                })
                .Where(x => x.message.Item2.Message.Text == "/checklivestream")
                .Select(x =>
                {

                    return LivestreamMessage.Create(new StreamLiveInformation((x.user?.Id ?? -1).ToString(), x.message.Item2.Message.From.Username, "Telegram", true));
                });


            var fromUser = DefinedMessageContract
                .ToMessages(commandMessages)
                .Merge(LivestreamContract.ToMessages(liveStreamMessages))
                ;

            return Observable.Merge(fromUser, 
                GetEmptyFrom(pluginMessageWithGroups), 
                GetEmptyFrom(incommingLivestreamMessages), 
                GetEmptyFrom(incommingBetterplaceMessages), 
                GetEmptyFrom(incommingCommandStream));
        }


        private static HashSet<long> GetIdsOfGroup(string groupname)
        {
            HashSet<long> userIds = new();
            using var ctx = new RightsDbContext();
            var livestreamgroup = ctx.Groups.FirstOrDefault(x => x.Name == groupname);


            if (livestreamgroup is not null)
            {
                foreach (var item in livestreamgroup.PlattformUsers)
                {
                    if (item.Platform != "Telegram")
                        continue;
                    userIds.Add(long.Parse(item.PlattformUserId));
                }
            }
            return userIds;
        }

        private static Dictionary<string, string> commandDescriptions = new Dictionary<string, string>() {
            {"start", "Start the bot interactions"},
            {"connect", "Connect two accounts of different plattforms" },
            {"subscribe", "Subscribe to a group of your liking" },
            {"unsubscribe", "Unsubscribe from a previous subscribed group" },
            {"crash", "Crashes the bot for test cases" },
        };
        private static void CreateIncommingCommandCallbacks(TelegramContext botContext)
        {
            var commands = CommandoCentral.GetCommandsFor("Telegram");
            foreach (var item in commands)
            {

                botContext.AddCommand(x => SimpleCommands.SendTextCommand(x, item, botContext), item.Command);
            }
            botContext.AddCommand(x => SimpleCommands.Start(x, botContext), "start");
            botContext.AddCommand(x => SimpleCommands.Connect(botContext, x), "connect");
            botContext.AddCommand(x => SimpleCommands.Subscribe(botContext, x), "subscribe");
            botContext.AddCommand(x => SimpleCommands.Unsubscribe(botContext, x), "unsubscribe");
#if DEBUG
            botContext.AddCommand(x => throw new Exception("Test"), "crash");
#endif
            botContext.Client.SetMyCommandsAsync(botContext.CommandoCentral.Commands.Select(x => new BotCommand() { Command = x.Command, Description = commandDescriptions.ContainsKey(x.Command) ? commandDescriptions[x.Command] : "_" })).GetAwaiter().GetResult();
        }

        private static IObservable<(string, TelegramCommandArgs)> CreateCommands(TelegramBotClient client) =>
            StartReceivingMessageUpdates(client)
                    .Select(args => args.Message)
                    .Where(message => message.Type == MessageType.Text && !string.IsNullOrWhiteSpace(message.Text))
                    .Do(message => logger.Debug($"Got Message: {message.Text}"))
                    .Where(message => message.Text.StartsWith('/'))
                    .Select(message => (message.Text.TrimStart('/').ToLower(), new TelegramCommandArgs(message, client)))
                    .Publish()
                    .RefCount();

        private static IObservable<Update> StartReceivingMessageUpdates(TelegramBotClient botClient)
        {
            var limit = 0;
            var emptyUpdates = Array.Empty<Update>();
            var timeout = botClient.Timeout.TotalSeconds;

            IObservable<Update[]> RequestUpdate(UpdateContext updateContext)
            {
                return Observable
                    .Defer(() =>
                    {
                        var request
                            = new GetUpdatesRequest
                            {
                                Limit = limit,
                                Offset = updateContext.MessageOffset,
                                Timeout = (int)timeout,
                                AllowedUpdates = new[] { UpdateType.Message }
                            };

                        return Observable
                                .FromAsync(token => botClient.MakeRequestAsync(request: request, cancellationToken: token));
                    }
                    )
                    .Where(x => x.Length > 0)
                    .Do(updates => updateContext.MessageOffset = updates[^1].Id + 1);
            }


            return
                Observable
                .Using(
                    () => new UpdateContext(0),
                    context =>
                        RequestUpdate(context)
                        .Repeat()
                        .SelectMany(u => u)
                );
        }

        public class UpdateContext : IDisposable
        {
            public UpdateContext(int offset)
            {
                MessageOffset = offset;
            }

            public int MessageOffset { get; set; }

            public void Dispose()
            {
            }
        }
    }
}
