
using BotMaster.Commandos;
using BotMaster.MessageContract;
using BotMaster.PluginSystem.Messages;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Database;
using BotMaster.Twitch.Commands;
using BotMaster.Twitch.MessageContract;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

using DefinedContract = BotMaster.MessageContract.Contract;
using TwitchContract = BotMaster.Twitch.MessageContract.Contract;

namespace BotMaster.Twitch
{
    public class Bot
    {
        public string ChannelId { get; private set; }

        public const string SourcePlattform = "Twitch";

        internal BotCommandManager Manager;

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

                    context.AddCommand((c) => client.SendMessage(context.Channel, $"[{c.Username}]: {string.Join(' ', c.Parameter)}"), SourcePlattform);
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

                    var commands = CommandoCentral.GetCommandsFor("Telegram");
                    foreach (var item in commands)
                    {
                        context.CommandoCentral.AddCommand(x => SimpleCommands.SendTextCommand(x, item, context), item.Command);
                    }

                    var messages = Observable
                        .FromEventPattern<OnConnectedArgs>(add => client.OnConnected += add, remove => client.OnConnected -= remove)
                        .Select(e =>
                        {
                            var incommingDefinedMessages = DefinedContract
                               .ToDefineMessages(notifications)
                               .VisitMany(
                                    textMessage => Observable.Empty<DefinedMessage>(),
                                    commandMessage => context.CommandoCentral.CreateCommandStream(commandMessage)
                                        .Select(x => (DefinedMessage)x),
                                    chatMessage => chatMessage
                                        .Where(x => x.Source != SourcePlattform)
                                        .Do(message => client.SendMessage(context.Channel, $"[{message.Username}]: {message.Text}"))
                                        .Select(x => (DefinedMessage)x)
                                   );

                            var incommingTwitchMessages = TwitchContract
                               .ToDefineMessages(notifications)
                               .VisitMany(
                                    follower => follower
                                        .Do(x => client.SendMessage(context.Channel, $"{x.UserName} hat sich verklickt. Vielen lieben Dank dafür <3"))
                                        .Select(x => (TwitchMessage)x),
                                    raid => raid
                                        .Do(message => client.SendMessage(context.Channel, $"{message.UserName} bringt jede Menge Noobs mit, nämlich 1 bis {message.Count}. Yippie"))
                                        .Select(x => (TwitchMessage)x)
                                   );

                            var raidInfo = Observable
                            .FromEventPattern<OnRaidNotificationArgs>(add => client.OnRaidNotification += add, remove => client.OnRaidNotification -= remove)
                            .Select(x => x.EventArgs)
                            .Select(e =>
                            {
                                if (!int.TryParse(e.RaidNotification.MsgParamViewerCount, out int count))
                                    count = 0;
                                return (TwitchMessage)new RaidInformation(e.RaidNotification.MsgParamDisplayName, count, e.RaidNotification.SystemMsgParsed);
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
                                .Select(x => (TwitchMessage)x);

                            var twitchMessages = TwitchContract.ToMessages(raidInfo.Merge(follower));
                            var definedMessages = DefinedContract.ToMessages(commandMessages.Merge(chatMessages));
                            var pnS = DefinedContract.ToMessages(privateCommandMessages);

                            return Observable.Using(
                                () => StableCompositeDisposable.Create(
                                    incommingDefinedMessages.Subscribe(),
                                    incommingTwitchMessages.Subscribe()),
                                (_) => Observable.Merge(twitchMessages, definedMessages, pnS));
                        })
                        .Merge();

                    if (!context.Client.Connect())
                        throw new HttpRequestException();

                    t.ThrowIfCancellationRequested();

                    return Task.FromResult(messages);
                }
            );
        }

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

            return new TwitchContext(api, client, userId, channelName, commandoCentral, new());
        }

    }
}
