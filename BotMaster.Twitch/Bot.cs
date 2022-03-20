
using BotMaster.MessageContract;
using BotMaster.PluginSystem.Messages;
using BotMaster.Twitch.MessageContract;

using Newtonsoft.Json;

using NLog;

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

namespace WarFabrik.Clone
{
    public class Bot
    {
        public string ChannelId { get; private set; }

        internal BotCommandManager Manager;


        public static IObservable<Message> Create(TokenFile tokenFile, AccessToken accessToken, string channelName, IObservable<Message> notifications)
        {
            return Observable
                .Using((t) => CreateContext(tokenFile, accessToken, channelName, t), (context, t) =>
                {
                    t.ThrowIfCancellationRequested();
                    var client = context.Client;

                    var messages = Observable
                        .FromEventPattern<OnConnectedArgs>(add => client.OnConnected += add, remove => client.OnConnected -= remove)
                        .Select(e =>
                        {
                            var incommingDefinedMessages = DefinedContract
                               .ToDefineMessages(notifications)
                               .VisitMany(
                                    textMessage => Observable.Empty<DefinedMessage>(),
                                    commandMessage => commandMessage
                                        .Where(message => message.Command == "twitch")
                                        .Do(x => client.SendMessage(context.Channel, string.Join(' ', x.Parameter)))
                                        .Select(x => (DefinedMessage)x),
                                    chatMessage => chatMessage
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
                                .Select(x => DefinedMessage.CreateChatMessage(x.ChatMessage.Username, x.ChatMessage.Message));


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
                                    var parameter = message[end..].Split(' ');

                                    return DefinedMessage.CreateCommandMessage(command, parameter);
                                });

                            var internalCommands
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
                                    var parameter = message[end..].Split(' ');

                                    return (command, new BotCommandArgs(null, context.Api, e.ChatMessage));
                                    //Manager.DispatchAsync(command, new BotCommandArgs(this, api, e.ChatMessage));
                                });

                            var follower = FollowerService
                                .GetFollowerUpdates(context.Api, context.UserId, TimeSpan.FromSeconds(10), Scheduler.Default)
                                .Select(x => (TwitchMessage)x);

                            var twitchMessages = TwitchContract.ToMessages(Observable.Merge(raidInfo, follower));
                            var definedMessages = DefinedContract.ToMessages(Observable.Merge(commandMessages, chatMessages));

                            return Observable.Using(
                                () => StableCompositeDisposable.Create(
                                    incommingDefinedMessages.Subscribe(),
                                    incommingTwitchMessages.Subscribe()),
                                (_) => Observable.Merge(twitchMessages, definedMessages));
                        })
                        .Merge();

                    if (!context.Client.Connect())
                        throw new HttpRequestException();

                    t.ThrowIfCancellationRequested();

                    return Task.FromResult(messages);
                }
            );
        }

        private static async Task<TwitchContext> CreateContext(TokenFile tokenFile, AccessToken accessToken, string channelName, CancellationToken t)
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

            return new TwitchContext(api, client, userId, channelName);
        }

        private record TwitchContext(TwitchAPI Api, TwitchClient Client, string UserId, string Channel) : IDisposable
        {
            public Logger Logger { get; } = LogManager.GetLogger($"{nameof(Bot)}_{UserId}");

            public void Dispose()
            {
            }
        }
    }
}
