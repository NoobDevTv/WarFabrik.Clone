using BotMaster.Betterplace.MessageContract;
using BotMaster.Commandos;
using BotMaster.MessageContract;
using BotMaster.PluginSystem.Messages;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Database;
using BotMaster.Twitch.MessageContract;
using BotMaster.YouTube.Commands;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

using Microsoft.EntityFrameworkCore;

using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;


using DefinedContract = BotMaster.MessageContract.Contract;
using TwitchContract = BotMaster.Twitch.MessageContract.TwitchContract;

namespace BotMaster.YouTube
{
    public class Bot
    {
        public string ChannelId { get; private set; }

        public const string SourcePlattform = YoutubeContext.Plattform;

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

        public static IObservable<Message> Create(IObservable<Message> notifications)
        {
            return
                Observable
                .Using(() => CreateContext(new CommandoCentral()), (context) =>
                {
                    var client = context.Client;
                    CreateCommands(context, client);

                    var subscriptions = SubscribeExternMessages(notifications, context, client);

                    IObservable<Message> messages = GetChatMessages(context);

                    var follower = YoutubeServiceInformation
                        .GetFollowerUpdates(context.Api, context.MetaData, TimeSpan.FromSeconds(60), Scheduler.Default)
                        .Select(x => (TwitchMessage)x);

                    var twitchMessages = TwitchContract.ToMessages(follower).Merge(messages);

                    return Observable.Using(
                                    () => subscriptions,
                                    (_) => twitchMessages);
                }
            );
        }

        private static IObservable<Message> GetChatMessages(YoutubeContext context)
        {
            return YoutubeServiceInformation
                    .GetBroadcasts(context.Api, context.MetaData, TimeSpan.FromMinutes(2), Scheduler.Default)
                    .Select(e =>
                    {
                        if (context.Client.CurrentBroadcast is not null)
                            context.Client.CurrentBroadcast.ActualEndTime = DateTime.Now;

                        if (e is null || e.Count == 0)
                        {
                            context.Client.CurrentBroadcast = null;
                            return Observable.Empty<Message>();
                        }

                        var first = e.First()!;
                        context.Client.CurrentBroadcast = first.Snippet;

                        var messages = YoutubeServiceInformation
                                .GetMessageUpdates(context.Api, context.MetaData, TimeSpan.FromSeconds(5), first.Snippet, Scheduler.Default)
                                .Where(e => e.ChatMessage.Username != context.Channel.Snippet.Title)
                            ;

                        var chatMessages
                            = messages
                            .Where(x => !x.ChatMessage.Text.Contains('!'))
                            .Select(x => DefinedMessage.CreateChatMessage(x.ChatMessage.Username, x.ChatMessage.Text, SourcePlattform));


                        var commandMessages
                            = messages
                            .Where(x => x.ChatMessage.Text.Contains('!'))
                            .Select(e =>
                            {
                                var message = e.ChatMessage.Text;
                                var index = message.IndexOf('!');
                                var end = message.IndexOf(' ', index);

                                if (end < 1)
                                    end = message.Length;

                                var command = message[index..end].Trim().TrimStart('!').ToLower();
                                var parameter = message[end..].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                                var user = GetUser(SourcePlattform, e.ChannelId);

                                return DefinedMessage.CreateCommandMessage(command, e.ChatMessage.Username, user?.Id ?? -1, e.ChannelId, SourcePlattform, false, parameter);
                            });

                        var definedMessages = DefinedContract.ToMessages(commandMessages.Merge(chatMessages));

                        return definedMessages;

                    })
                    .Concat();
        }

        private static IDisposable SubscribeExternMessages(IObservable<Message> notifications, YoutubeContext context, YoutubeClient client)
        {
            var incommingDefinedMessages = DefinedContract
               .ToDefineMessages(notifications)
               .VisitMany(
                    textMessage => Observable.Empty<DefinedMessage>(),
                    commandMessage => context.CommandoCentral.CreateCommandStream(commandMessage)
                        .Select(x => (DefinedMessage)x),
                    chatMessage => chatMessage
                        .Where(x => x.Source != SourcePlattform)
                        .Do(message => client.SendMessage($"[{message.Username}]: {message.Text}"))
                        .Select(x => (DefinedMessage)x)
                   );

            var incommingTwitchMessages = TwitchContract
               .ToDefineMessages(notifications)
               .VisitMany(
                    follower => follower
                        .Do(x => client.SendMessage($"{x.UserName} hat sich verklickt. Vielen lieben Dank dafür <3"))
                        .Select(x => (TwitchMessage)x),
                    raid => raid
                        .Do(message => client.SendMessage($"{message.UserName} bringt jede Menge Noobs mit, nämlich 1 bis {message.Count}. Yippie"))
                        .Select(x => (TwitchMessage)x)
                   );

            var incommingBetterplaceMessages = BetterplaceContract
                  .ToDefineMessages(notifications)
                  .VisitMany(
                       donation => donation
                           .Do(x => client.SendMessage($"{x.Author} hat {x.Donated_amount_in_cents} Geld gespendet. Vielen lieben Dank dafür <3"))
                           .Select(x => (BetterplaceMessage)x)
                      );

            return StableCompositeDisposable.Create(
                                         incommingDefinedMessages.Subscribe(),
                                         incommingBetterplaceMessages.Subscribe(),
                                         incommingTwitchMessages.Subscribe());
        }

        private static void CreateCommands(YoutubeContext context, YoutubeClient client)
        {

            context.AddCommand((c) => SimpleCommands.Twitch(context, c), "twitch");
            context.AddCommand((c) => SimpleCommands.Hype(context, c), "hype");
            context.AddCommand((c) => SimpleCommands.Uptime(context, c), "uptime");
            context.AddCommand((c) => SimpleCommands.Help(context, c), "?", "help");
            context.AddCommand((c) => SimpleCommands.TelegramGroup(context, c), "telegram");
            context.AddCommand((c) => SimpleCommands.FlipACoin(context, c), "flipacoin");
            context.AddCommand((c) => SimpleCommands.Github(context, c), "github");
            context.AddCommand((c) => SimpleCommands.TeamSpeak(context, c), "teamspeak", "ts");
            context.AddCommand((c) => SimpleCommands.Twitter(context, c), "twitter");
            context.AddCommand((c) => SimpleCommands.Donate(context, c), "donate");
            //context.AddCommand((c) => SimpleCommands.Youtube(context, c), "youtube", "yt");
            context.AddCommand((c) => SimpleCommands.Discord(context, c), "discord", "dc");
            context.AddCommand((c) => SimpleCommands.Time(context, c), "time");
            context.AddCommand((c) => SimpleCommands.Streamer(context, c), "streamer");
            context.AddCommand((c) => SimpleCommands.Project(context, c), "projects");
            //context.AddCommand((c) => SimpleCommands.Register(context, c), "register");
            //context.AddCommand((c) => c.Secure, (c) => SimpleCommands.PrivateConnect(context, c), "connect");
            context.AddCommand((c) => !c.Secure, (c) => SimpleCommands.PublicConnect(context, c), "connect");

            context.AddCommand((c) => GetUser(c.SourcePlattform, c.PlattformUserId)?.HasRight("AddCommand") ?? false && c.SourcePlattform == SourcePlattform, (c) => SimpleCommands.Add(context, c), "add");

            var commands = CommandoCentral.GetCommandsFor("Twitch");
            foreach (var item in commands)
            {
                context.AddCommand(x => SimpleCommands.SendTextCommand(x, item, context), item.Command);
            }
        }

        private static YoutubeContext CreateContext(CommandoCentral commandoCentral)
        {

            var metaData = Newtonsoft.Json.JsonConvert.DeserializeObject<YoutubeMetaData>(File.ReadAllText("additionalfiles/youtube_metadata.json"));
            using var stream = File.OpenRead("additionalfiles/client_secret.json");
            var token = new TokenResponse { RefreshToken = metaData.Refresh_Token };

            var credential = new UserCredential(
                new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = GoogleClientSecrets.Load(stream).Secrets
                    }),
                metaData.User_Id,
                token
                );

            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = metaData.Application_Name
            });

            var channelReq = youtubeService.Channels.List("snippet");
            channelReq.Id = metaData.Channel_Id;

            var noobDevChannel = channelReq.Execute().Items.First();

            return new YoutubeContext(youtubeService, new(youtubeService), metaData, noobDevChannel, commandoCentral, new());
        }

    }
}
