using BotMaster.Core.NLog;
using BotMaster.Database.Model;
using BotMaster.MessageContract;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;

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

namespace NoobDevBot.Telegram
{
    internal class TelegramService : Plugin
    {
        private readonly IMessageContractInfo[] messageContracts;

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
                            Create(MessageConvert.ToMessage(receivedPackages), botInstance.Client)
                        )
            );

        private BotInstance CreateBot()
        {
            var client = new TelegramBotClient("");
            return new(client);
        }

        private record BotInstance(TelegramBotClient Client) : IDisposable
        {
            public void Dispose()
            {
            }
        }

        //TODO: var info = new FileInfo(Path.Combine(".", "additionalfiles", "Telegram_Token.txt"));
        //TODO: Handle commands  
        public static IObservable<PluginMessage> Create(IObservable<PluginMessage> notifications, TelegramBotClient client)
        {
            Logger logger = LogManager.GetCurrentClassLogger();


            //Messages from other plugins
            IObservable<(Group, TextMessage n)> pluginMessageWithGroups
                = DefinedMessageContract
                    .ToDefineMessages(notifications)
                    .Match<TextMessage>(n => n)
                    .Select(n => (DatabaseManager.GetGroupByName("NoobDev"), n));

            var textMessages = SendMessageToGroup(pluginMessageWithGroups, logger, client);

            //Messages from User from telegram
            IObservable<(string, TelegramCommandArgs)> chatMessages = CreateCommands(client);

            var commandMessages = chatMessages
              .Select(x => (x.Item2.Message.Text, x.Item2.Message.Text.Split(' ')[1..]))
              .Select(x => DefinedMessage.CreateCommandMessage(x.Text, x.Item2));

            var incomming = DefinedMessageContract.ToMessages(commandMessages);


            return Observable.Using(() => textMessages.Subscribe(), _ => incomming);
        }


        private static IObservable<(string, TelegramCommandArgs)> CreateCommands(TelegramBotClient client) =>
            StartReceivingMessageUpdates(client, TimeSpan.FromSeconds(30))
                    .Select(args => args.Message)
                    .Where(message => message.Type == MessageType.Text && !string.IsNullOrWhiteSpace(message.Text))
                    .Where(message => message.Text.StartsWith('/'))
                    .Select(message => (message.Text.TrimStart('/').ToLower(), new TelegramCommandArgs(message, client)));

        private static IObservable<TelegramMessage> SendMessageToGroup(IObservable<(Group Group, TextMessage Message)> groupMessages,
            ILogger logger, TelegramBotClient client) =>
            groupMessages
                    .Select(messages => (messages.Group.User, messages.Message))
                    .Select(m => m
                                .User
                                .Select(x => client.SendTextMessageAsync(new ChatId(x.Name), m.Message.Text).ToObservable())
                                .Concat())
                    .Concat()
                    .OnError(logger, ex => $"Error on {nameof(SendMessageToGroup)}: {ex.Message}");

        private static IObservable<Update> StartReceivingMessageUpdates(TelegramBotClient botClient, TimeSpan pollingInterval)
        {
            var limit = 0;
            var emptyUpdates = Array.Empty<Update>();
            var timeout = botClient.Timeout.TotalSeconds;

            return
                Observable
                .Using(
                    () => new UpdateContext(0),
                    context =>
                        Observable
                            .Interval(pollingInterval)
                            .Select(_ =>
                                new GetUpdatesRequest
                                {
                                    Limit = limit,
                                    Offset = context.MessageOffset,
                                    Timeout = (int)timeout,
                                    AllowedUpdates = new[] { UpdateType.Message }
                                }
                            )
                            .Select(request =>
                                Observable
                                    .FromAsync(token =>
                                        botClient.MakeRequestAsync(request: request, cancellationToken: token)
                                    )
                                    .Do(updates => context.MessageOffset = updates[^0].Id + 1)
                            )
                            .Concat()
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
