using BotMaster.Core.NLog;
using BotMaster.Database.Model;
using BotMaster.MessageContract;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using BotMaster.Telegram.Database;

using NLog;

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
    internal class TelegramService : Plugin
    {
        private readonly IMessageContractInfo[] messageContracts;
        private static Logger logger;

        public TelegramService()
        {
        }

        public override IObservable<Package> Start(IObservable<Package> receivedPackages)
            => Observable
            .Using(
                CreateBot,
                botInstance
                    => MessageConvert
                        .ToPackage(
                            Create(MessageConvert.ToMessage(receivedPackages), botInstance.Client))
            );

        private BotInstance CreateBot()
        {
            var info = new FileInfo(Path.Combine(".", "additionalfiles", "Token.txt"));
            if (!info.Directory.Exists)
                info.Directory.Create();

            var token = System.IO.File.ReadAllText(info.FullName);
            var client = new TelegramBotClient(token);
            return new(client);
        }

        private record BotInstance(TelegramBotClient Client) : IDisposable
        {
            public void Dispose()
            {
            }
        }

        //TODO: Handle commands  
        public static IObservable<PluginMessage> Create(IObservable<PluginMessage> notifications, TelegramBotClient client)
        {
            logger = LogManager.GetCurrentClassLogger();

            using var context = new TelegramContext();
            context.Database.EnsureCreated();
            var telegramUsers
                = context.Set<Group>()
                    .FirstOrDefault(x => x.Name == "NoobDev")
                    ?.Users
                    .Join(context.TelegramUsers, x => x.Id, x => x.User.Id, (_, user) => user.Id)
                    .ToList();

            //Messages from other plugins
            IObservable<(List<long>, TextMessage n)> pluginMessageWithGroups
                = DefinedMessageContract
                    .ToDefineMessages(notifications)
                    .Log(logger, nameof(TelegramService) + " Incomming", onNext: LogLevel.Debug)
                    .Match<TextMessage>(n => n)
                    .Select(n => (telegramUsers, n));

            foreach (var id in telegramUsers)
            {
                //_ = client.SendTextMessageAsync(new ChatId(id), "New Bot started :)");
            }

            var textMessages = SendMessageToGroup(pluginMessageWithGroups, logger, client);

            //Messages from User from telegram
            IObservable<(string, TelegramCommandArgs)> chatMessages = CreateCommands(client);

            var commandMessages = chatMessages
              .Select(x =>
              {
                  string[] split = x.Item2.Message.Text.TrimStart('/').Split(' ');
                  return DefinedMessage.CreateCommandMessage(split[0].ToLower(), split[1..]);
              });

            var incomming = DefinedMessageContract.ToMessages(commandMessages);

            return Observable.Using(() => textMessages.Subscribe(), _ => incomming);
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
