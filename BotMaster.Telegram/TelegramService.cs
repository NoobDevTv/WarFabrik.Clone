using BotMaster.Commandos;
using BotMaster.Core.NLog;
using BotMaster.MessageContract;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Commands;
using BotMaster.Telegram.Database;

using Microsoft.EntityFrameworkCore;

using NLog;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using DefinedMessageContract = BotMaster.MessageContract.Contract;
using MessageType = Telegram.Bot.Types.Enums.MessageType;
using PluginMessage = BotMaster.PluginSystem.Messages.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace BotMaster.Telegram
{
    internal partial class TelegramService : Plugin
    {
        private readonly IMessageContractInfo[] messageContracts;
        private static Logger logger;

        public TelegramService()
        {
        }

        public override IObservable<Package> Start(ILogger logger, IObservable<Package> receivedPackages)
        {
            using (var ctx = new RightsDbContext())
                ctx.Database.Migrate();
            using (var ctx = new UserConnectionContext())
                ctx.Database.Migrate();

            return Observable
            .Using(
                CreateBot,
                botInstance
                    => MessageConvert
                        .ToPackage(
                            Create(MessageConvert.ToMessage(receivedPackages), botInstance))
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

            //Messages from other plugins
            IObservable<(List<long>, TextMessage n)> pluginMessageWithGroups
                = definedMessages
                    .Match<TextMessage>(n => n) //TODO What about ChatMessage?!
                    .Select(n => (noobDevGroupUser, n));

            var incommingCommandStream = botContext.CommandoCentral.CreateCommandStream(definedMessages
                .Match<CommandMessage>(n => n));


            foreach (var id in noobDevGroupUser)
            {
                _ = client.SendTextMessageAsync(new ChatId(id), "New Bot started :)");
            }

            var textMessages = SendMessageToGroup(pluginMessageWithGroups, logger, client);

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
                //.Where(x => x.user is not null || x.plattformUser is not null)
                .Select(x =>
                {
                    string[] split = x.message.Item2.Message.Text.TrimStart('/').Split(' ');

                    return DefinedMessage.CreateCommandMessage(split[0].ToLower(), x.message.Item2.Message.From.Username, x.user?.Id ?? -1, x.plattformUser?.PlattformUserId ?? x.message.Item2.Message.From.Id.ToString(), "Telegram", true, split[1..]);
                });

            var fromUser = DefinedMessageContract.ToMessages(commandMessages);

            return Observable.Using(() => StableCompositeDisposable.Create(textMessages.Subscribe(), incommingCommandStream.Subscribe()), _ => fromUser);
        }


        private static void CreateIncommingCommandCallbacks(TelegramContext botContext)
        {
            var commands = CommandoCentral.GetCommandsFor("Telegram");
            foreach (var item in commands)
            {
                botContext.CommandoCentral.AddCommand(x => SimpleCommands.SendTextCommand(x, item, botContext), item.Command);
            }
            botContext.CommandoCentral.AddCommand(x => SimpleCommands.Start(x, botContext), "start");
            botContext.CommandoCentral.AddCommand(x => SimpleCommands.Connect(botContext, x), "connect");
        }

        private static IObservable<(string, TelegramCommandArgs)> CreateCommands(TelegramBotClient client) =>
            StartReceivingMessageUpdates(client)
                    .Select(args => args.Message)
                    .Where(message => message.Type == MessageType.Text && !string.IsNullOrWhiteSpace(message.Text))
                    .Do(message => logger.Debug($"Got Message: {message.Text}"))
                    .Where(message => message.Text.StartsWith('/'))
                    .Select(message => (message.Text.TrimStart('/').ToLower(), new TelegramCommandArgs(message, client)));

        private static IObservable<TelegramMessage> SendMessageToGroup(IObservable<(List<long> userIds, TextMessage Message)> groupMessages,
            ILogger logger, TelegramBotClient client)
        {
            return groupMessages
                .Select(m => m
                            .userIds
                            .Select(x => client.SendTextMessageAsync(new ChatId(x), m.Message.Text).ToObservable())
                            .Concat())
                .Concat()
                .OnError(logger, nameof(SendMessageToGroup), ex => $"{ex.Message}\n{ex.StackTrace}");
        }

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
