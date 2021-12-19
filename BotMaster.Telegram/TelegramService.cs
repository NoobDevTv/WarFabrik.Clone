using BotMaster.Core.NLog;
using BotMaster.Database.Model;
using BotMaster.MessageContract;
using NLog;
using NLog.Fluent;
using NoobDevBot.Telegram.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using MessageType = Telegram.Bot.Types.Enums.MessageType;
using PluginMessage = BotMaster.PluginSystem.Messages.Message;
using System.Threading;
using Telegram.Bot.Requests;
using System.Reactive;
using Telegram.Bot.Extensions.Polling;
using Newtonsoft.Json.Linq;
using BotMaster.PluginSystem;
using BotMaster.Betterplace.MessageContract;
using BotMaster.PluginSystem.Messages;

namespace NoobDevBot.Telegram
{
    internal class TelegramService : Plugin
    {
        private readonly IMessageContractInfo[] messageContracts;

        public TelegramService()
        {
            messageContracts = new[]
            {
                (IMessageContractInfo)BetterplaceMessageContractInfo.Create()
            };
        }

        public override IObservable<Package> Start(IObservable<Package> receivedPackages)
            => Observable
            .Using(
                CreateBot,
                botInstance
                    => MessageConvert
                        .ToPackage(
                            Observable
                            .FromAsync(token => botInstance.Client.Run(token))
                            .SelectMany(_ => GetMessage(botInstance.NewFollower, botInstance.Raids))
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
        public static IObservable<PluginMessage> Create(IObservable<PluginMessage> notifications)
            => Observable.Create<PluginMessage>(observer =>
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                
                IObservable<(string, TelegramCommandArgs)> commands = CreateCommands(client);
                IObservable<(Group, TextMessage n)> adminMessages = notifications
                                    .OfType<TextMessage>()
                                    .Where(n => n.Type.Administrative)
                                    .Select(n => (DatabaseManager.GetGroupByName("NoobDev"), n));

                var updates = ;
                IDisposable disposable = SendMessageToGroup(adminMessages, logger, client);

                return new CompositeDisposable { disposable, Disposable.Create(() => client.StopReceiving()) };
            });

        private static IObservable<(string, TelegramCommandArgs)> CreateCommands(TelegramBotClient client) =>
            StartReceivingMessageUpdates(client, TimeSpan.FromSeconds(30))
                    .Select(args => args.Message)
                    .Where(message => message.Type == MessageType.Text && !string.IsNullOrWhiteSpace(message.Text))
                    .Where(message => message.Text.StartsWith('/'))
                    .Select(message => (message.Text.TrimStart('/').ToLower(), new TelegramCommandArgs(message, client)));

        private static IDisposable SendMessageToGroup(IObservable<(Group Group, TextMessage Message)> groupMessages,
            ILogger logger, TelegramBotClient client) =>
            groupMessages
                   .Select(messages => (messages.Group.User, messages.Message))
                   .Do(m => m
                             .User
                             .Select(x => client.SendTextMessageAsync(x.ChatId, m.Message.Content))
                             .Select(t => t.ConfigureAwait(false).GetAwaiter())
                             .ForEach(a => a.GetResult())
                       )
                   .OnError(logger, ex => $"Error on {nameof(SendMessageToGroup)}: {ex.Message}")
                   .Subscribe();

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
