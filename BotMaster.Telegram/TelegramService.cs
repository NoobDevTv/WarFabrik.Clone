using BotMaster.Core.NLog;
using BotMaster.Core.Notifications;
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
using MessageNotification = BotMaster.Core.Notifications.MessageNotification;
using MessageType = Telegram.Bot.Types.Enums.MessageType;

namespace NoobDevBot.Telegram
{
    internal static class TelegramService
    {
        //TODO: var info = new FileInfo(Path.Combine(".", "additionalfiles", "Telegram_Token.txt"));
        //TODO: Handle commands  
        public static IObservable<Notification> Create(IObservable<Notification> notifications)
            => Observable.Create<Notification>(observer =>
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                var client = new TelegramBotClient("");
                IObservable<(string, TelegramCommandArgs)> commands = CreateCommands(client);
                IObservable<(Group, MessageNotification n)> adminMessages = notifications
                                    .OfType<MessageNotification>()
                                    .Where(n => n.Type.Administrative)
                                    .Select(n => (DatabaseManager.GetGroupByName("NoobDev"), n));

                IDisposable disposable = SendMessageToGroup(adminMessages, logger, client);
                client.StartReceiving();

                return new CompositeDisposable { disposable, Disposable.Create(() => client.StopReceiving()) };
            });

        private static IObservable<(string, TelegramCommandArgs)> CreateCommands(TelegramBotClient client) =>
            Observable
                    .FromEventPattern<MessageEventArgs>(a => client.OnMessage += a, a => client.OnMessage -= a)
                    .Select(args => args.EventArgs.Message)
                    .Where(message => message.Type == MessageType.Text && !string.IsNullOrWhiteSpace(message.Text))
                    .Where(message => message.Text.StartsWith('/'))
                    .Select(message => (message.Text.TrimStart('/').ToLower(), new TelegramCommandArgs(message, client)));

        private static IDisposable SendMessageToGroup(IObservable<(Group Group, MessageNotification Message)> groupMessages,
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
    }
}
