
using BotMaster.Betterplace.MessageContract;
using BotMaster.Commandos;
using BotMaster.MessageContract;
using BotMaster.PluginSystem.Messages;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Database;
using BotMaster.Twitch.Commands;
using BotMaster.Livestream.MessageContract;

using Microsoft.EntityFrameworkCore;

using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

using DefinedContract = BotMaster.MessageContract.Contract;
using LivestreamContract = BotMaster.Livestream.MessageContract.LivestreamContract;
using BotMaster.Core.NLog;
using NLog;

namespace BotMaster.Twitch
{
    public class Bot
    {
        public string ChannelId { get; private set; }

        public const string SourcePlattform = TwitchContext.Plattform;

        internal static PlattformUser? GetUser(string plattform, string plattformUserId)
        {
            using var context = new RightsDbContext();
            return context.PlattformUsers
                .Include(x => x.Rights)
                .Include(x => x.User)
                .Include(x => x.Groups)
                .FirstOrDefault(x => x.PlattformUserId == plattformUserId
                    && x.Platform == plattform);
        }

        public static IObservable<Message> Create(TokenFile tokenFile, AccessToken accessToken, string channelName, IObservable<Message> notifications)
        {
            return Observable
                .Using((t) => CreateContext(tokenFile, accessToken, channelName, t, new CommandoCentral()), (context, t) =>
                {
                    t.ThrowIfCancellationRequested();
                    var client = context.Client;

                    context.Logger.Debug("Starting to add all commands and subscriptions");
                    context.AddCommand((c) => SimpleCommands.Hype(context, c), "hype");
                    context.AddCommand((c) => SimpleCommands.Uptime(context, c), "uptime");
                    context.AddCommand((c) => SimpleCommands.Help(context, c), "?", "help");
                    context.AddCommand((c) => SimpleCommands.TelegramGroup(context, c), "telegram");
                    context.AddCommand((c) => SimpleCommands.FlipACoin(context, c), "flipacoin");
                    context.AddCommand((c) => SimpleCommands.Github(context, c), "github");
                    context.AddCommand((c) => SimpleCommands.TeamSpeak(context, c), "teamspeak", "ts");
                    context.AddCommand((c) => SimpleCommands.Twitter(context, c), "twitter");
                    context.AddCommand((c) => SimpleCommands.Donate(context, c), "donate");
                    context.AddCommand((c) => SimpleCommands.Youtube(context, c), "youtube", "yt");
                    context.AddCommand((c) => SimpleCommands.Discord(context, c), "discord", "dc");
                    context.AddCommand((c) => SimpleCommands.Time(context, c), "time");
                    context.AddCommand((c) => SimpleCommands.Streamer(context, c), "streamer");
                    context.AddCommand((c) => SimpleCommands.Project(context, c), "projects");
                    context.AddCommand((c) => SimpleCommands.Register(context, c), "register");
                    context.AddCommand((c) => c.Secure, (c) => SimpleCommands.PrivateConnect(context, c), "connect");
                    context.AddCommand((c) => !c.Secure, (c) => SimpleCommands.PublicConnect(context, c), "connect");

                    context.AddCommand((c) => GetUser(c.SourcePlattform, c.PlattformUserId)?.HasRight("AddCommand") ?? false && c.SourcePlattform == SourcePlattform, (c) => SimpleCommands.Add(context, c), "add");
                    context.AddCommand((c) => GetUser(c.SourcePlattform, c.PlattformUserId)?.HasRight("AddCommand") ?? false && c.SourcePlattform == SourcePlattform, (c) => SimpleCommands.Add(context, c, true), "addglobal");

                    var commands = CommandoCentral.GetCommandsFor("Twitch");
                    foreach (var item in commands)
                    {
                        context.AddCommand(x => SimpleCommands.SendTextCommand(x, item, context), item.Command);
                    }
                    var goneLiveMessages = LivestreamContract.ToMessages(Observable
                        .FromEventPattern<OnStreamUpArgs>(add => context.pubSub.OnStreamUp += add, remove => context.pubSub.OnStreamUp -= remove)
                        .Select(e => e.EventArgs)
                        .Select(e => (LivestreamMessage)new StreamLiveInformation(e.ChannelId, context.Channel, SourcePlattform, true))
                        );
                    var goneOfflineMessages = LivestreamContract.ToMessages(Observable
                        .FromEventPattern<OnStreamDownArgs>(add => context.pubSub.OnStreamDown += add, remove => context.pubSub.OnStreamDown -= remove)
                        .Select(e => e.EventArgs)
                        .Select(e => (LivestreamMessage)new StreamLiveInformation(e.ChannelId, context.Channel, SourcePlattform, false))
                        );

                    context.pubSub.ListenToVideoPlayback(context.UserId);



                    var messages = Observable
                        .FromEventPattern<OnConnectedArgs>(add => client.OnConnected += add, remove => client.OnConnected -= remove)
                        .Select(e =>
                        {

                            notifications = notifications
                                    .Log(context.Logger, nameof(TwitchService) + " Incomming", onNext: LogLevel.Debug)
                                    .Publish()
                                    .RefCount();

                            var incommingDefinedMessages = DefinedContract
                               .ToDefineMessages(notifications)
                               .VisitMany(
                                    textMessage => Observable.Empty<DefinedMessage>(),
                                    commandMessage => context.CommandoCentral.CreateCommandStream(commandMessage)
                                        .Where(x => x.SourcePlattform == SourcePlattform)
                                        .Select(x => (DefinedMessage)x),
                                    chatMessage => chatMessage
                                        .Where(x => x.Source != SourcePlattform)
                                        .Do(message => client.SendMessage(context.Channel, $"[{message.Username}]: {message.Text}"))
                                        .Select(x => (DefinedMessage)x)
                                )
                               .Log(context.Logger, "incommingDefinedMessages", subscription: LogLevel.Debug);

                            var incommingLivestreamMessages = LivestreamContract
                               .ToDefineMessages(notifications)
                               .VisitMany(
                                    follower => follower
                                        .Do(x => client.SendMessage(context.Channel, $"{x.UserName} hat sich verklickt. Vielen lieben Dank dafür <3"))
                                        .Select(x => (LivestreamMessage)x),
                                    raid => raid
                                        .Do(message => client.SendMessage(context.Channel, $"{message.UserName} bringt jede Menge Noobs mit, nämlich 1 bis {message.Count}. Yippie"))
                                        .Select(x => (LivestreamMessage)x),
                                    live => live
                                    .Where(x => x.SourcePlattform == SourcePlattform)
                                    .Do(message => client.SendMessage(context.Channel, $"Der Bot ist weiterhin online :)")).Select(x => (LivestreamMessage)x)
                               )
                               .Log(context.Logger, "incommingLivestreamMessages", subscription: LogLevel.Debug);

                            var incommingBetterplaceMessages = BetterplaceContract
                                  .ToDefineMessages(notifications)
                                  .VisitMany(
                                       donation => donation
                                           .Do(x => client.SendMessage(context.Channel, $"{x.Author} hat {x.Donated_amount_in_cents} Geld gespendet. Vielen lieben Dank dafür <3"))
                                           .Select(x => (BetterplaceMessage)x)
                                  )
                                  .Log(context.Logger, "incommingBetterplaceMessages", subscription: LogLevel.Debug);



                            var raidInfo = Observable
                            .FromEventPattern<OnRaidNotificationArgs>(add => client.OnRaidNotification += add, remove => client.OnRaidNotification -= remove)
                            .Select(x => x.EventArgs)
                            .Select(e =>
                            {
                                if (!int.TryParse(e.RaidNotification.MsgParamViewerCount, out int count))
                                    count = 0;
                                return (LivestreamMessage)new RaidInformation(e.RaidNotification.MsgParamDisplayName, count, e.RaidNotification.SystemMsgParsed);
                                /*client.SendMessage(initialChannel, $"{channel} bringt jede Menge Noobs mit, nämlich 1 bis {count}. Yippie");
                        OnRaid?.Invoke(this, e.RaidNotification.SystemMsgParsed);*/
                            });

                            var messages = Observable
                                .FromEventPattern<OnMessageReceivedArgs>(add => client.OnMessageReceived += add, remove => client.OnMessageReceived -= remove)
                                .Select(x => x.EventArgs)
                                .Where(e => e.ChatMessage.Username != client.TwitchUsername && !e.ChatMessage.IsMe)
                                ;


                            var chatMessages
                                = messages
                                .Where(x => !x.ChatMessage.Message.Contains('!'))
                                .Select(x => DefinedMessage.CreateChatMessage(x.ChatMessage.Username, x.ChatMessage.Message, SourcePlattform));



                            var commandMessages
                                = messages
                                .Where(x => x.ChatMessage.Message.Contains('!'))
                                .Select(e =>
                                {
                                    var message = e.ChatMessage.Message;
                                    var index = message.IndexOf('!');
                                    var end = message.IndexOf(' ', index);

                                    if (end < 1)
                                        end = message.Length;

                                    var command = message[index..end].Trim().TrimStart('!').ToLower();
                                    var parameter = message[end..].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                                    var user = GetUser(SourcePlattform, e.ChatMessage.UserId);

                                    return DefinedMessage.CreateCommandMessage(command, e.ChatMessage.Username, user?.Id ?? -1, e.ChatMessage.UserId, SourcePlattform, false, parameter);
                                });

                            var privateMessages = Observable
                             .FromEventPattern<OnWhisperReceivedArgs>(add => client.OnWhisperReceived += add, remove => client.OnWhisperReceived -= remove)
                             .Select(x => x.EventArgs)
                             .Where(e => e.WhisperMessage.Username != client.TwitchUsername)
                             ;

                            var privateCommandMessages
                                = privateMessages
                                .Where(x => x.WhisperMessage.Message.Contains('!'))
                                .Select(e =>
                                {
                                    var message = e.WhisperMessage.Message;
                                    var index = message.IndexOf('!');
                                    var end = message.IndexOf(' ', index);

                                    if (end < 1)
                                        end = message.Length;

                                    var command = message[index..end].Trim().TrimStart('!').ToLower();
                                    var parameter = message[end..].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                                    var user = GetUser(SourcePlattform, e.WhisperMessage.UserId);

                                    return DefinedMessage.CreateCommandMessage(command, e.WhisperMessage.Username, user?.Id ?? -1, e.WhisperMessage.UserId, SourcePlattform, true, parameter);
                                });



                            var follower = FollowerService
                                .GetFollowerUpdates(context.Api, context.UserId, TimeSpan.FromSeconds(10), Scheduler.Default)
                                .Select(x => (LivestreamMessage)x);

                            var LivestreamMessages = LivestreamContract.ToMessages(raidInfo.Merge(follower));
                            var definedMessages = DefinedContract.ToMessages(commandMessages.Merge(chatMessages));
                            var pnS = DefinedContract.ToMessages(privateCommandMessages);


                            return Observable.Merge(
                                LivestreamMessages, 
                                definedMessages, 
                                pnS,
                                GetEmptyFrom(incommingDefinedMessages),
                                GetEmptyFrom(incommingLivestreamMessages),
                                GetEmptyFrom(incommingBetterplaceMessages));

                        })
                        .Merge()
                        ;

                    if (!context.Client.Connect())
                        throw new HttpRequestException();

                    t.ThrowIfCancellationRequested();

                    return Task.FromResult(Observable.Merge(messages, goneOfflineMessages, goneLiveMessages));
                }
            );
        }
        protected static IObservable<Message> GetEmptyFrom<T>(IObservable<T> observe) => observe.IgnoreElements().Select(_ => Message.Empty);

        private static async Task<TwitchContext> CreateContext(TokenFile tokenFile, AccessToken accessToken, string channelName, CancellationToken t, CommandoCentral commandoCentral)
        {
            t.ThrowIfCancellationRequested();

            var api = new TwitchAPI();
            api.Settings.ClientId = tokenFile.ClientId;
            api.Settings.AccessToken = accessToken.Token;
            var credentials = new ConnectionCredentials(tokenFile.Name, tokenFile.OAuth);

            var client = new TwitchClient();
            client.Initialize(credentials, channel: channelName, autoReListenOnExceptions: true);

            var userResponse = await api.Helix.Users.GetUsersAsync(logins: new List<string> { channelName });
            var userId = userResponse.Users.First().Id;


            var pubSub = new TwitchPubSub();

            var ctx = new TwitchContext(api, client, pubSub, userId, channelName, commandoCentral, new());
            ctx.Logger.Debug("Created a new twitch context");
            return ctx;
        }

    }
}
