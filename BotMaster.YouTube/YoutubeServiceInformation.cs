using BotMaster.Core.NLog;
using BotMaster.MessageContract;
using BotMaster.Livestream.MessageContract;

using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using NLog;

using System.Reactive.Concurrency;
using System.Reactive.Linq;

using static Google.Apis.YouTube.v3.SearchResource.ListRequest;
using BotMaster.Core.Extensibility;

namespace BotMaster.YouTube
{
    public class YoutubeServiceInformation
    {
        public static IObservable<FollowInformation> GetFollowerUpdates(YouTubeService api, YoutubeMetaData metaData, TimeSpan period, IScheduler scheduler)
        {
            return Observable.Using(() => new FollowerServiceContext(api, metaData), serviceContext =>
           {
               var interval
               = Observable.Return(0L).Concat(Observable.Interval(period, scheduler));

               return interval
                   .Select(_ => api.Subscriptions.List("subscriberSnippet,contentDetails,snippet"))
                   .Do(req => { req.MyRecentSubscribers = true; req.MaxResults = 10; })
                   .Select(req => req.Execute())
                   .OnError(serviceContext.Logger, nameof(api.Subscriptions))
                   .Retry(x=>true, period)
                   .Select(followerResponse => GetNewFollower(followerResponse.Items, serviceContext.CurrentFollowers))
                   .Do(x => serviceContext.CurrentFollowers.AddRange(x))
                   .Skip(1)
                   .SelectMany(x => x);
           });
        }

        private static IReadOnlyCollection<FollowInformation> GetNewFollower(IList<Subscription> follows, IReadOnlyCollection<FollowInformation> alreadyExistingFollower)
        {
            return follows
                .Select(follow => new FollowInformation(follow.SubscriberSnippet.Title, follow.SubscriberSnippet.ChannelId, follow.Snippet.PublishedAt!.Value, YoutubeContext.Plattform))
                .Except(alreadyExistingFollower)
                .ToList();
        }

        public static IObservable<IList<LiveBroadcast?>> GetBroadcasts(YouTubeService api, YoutubeMetaData metaData, TimeSpan liveStreamInterval, IObservable<StreamLiveInformation> incommingLiveInformations, IScheduler scheduler)
        {

            var liveStreamsInterval
                = Observable.Timer(TimeSpan.FromSeconds(5))
                .Concat(Observable.Interval(liveStreamInterval, scheduler))
                .Merge(incommingLiveInformations.Where(x=>x.SourcePlattform != Bot.SourcePlattform)
                        .Select(x=>0L)
                        .Delay(TimeSpan.FromSeconds(5)));

            return liveStreamsInterval
                 .Select(_ => api.LiveBroadcasts.List("snippet"))
                 .Do(x => { x.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active; x.MaxResults = 1; })
                 .Select(x => x.Execute())
                 .Do(x => Console.WriteLine("Requested LiveStream snippets"))
                 .Retry(x => true, TimeSpan.FromSeconds(10))
                 .DistinctUntilChanged(x => x.Items.FirstOrDefault()?.Snippet.LiveChatId)
                 .Select(x => x.Items)
                 .Publish()
                 .RefCount();
        }

        public static IObservable<YoutubeChatMessage> GetMessageUpdates(YouTubeService api, YoutubeMetaData metaData, TimeSpan messageInterval, LiveBroadcastSnippet liveBroadcast, IScheduler scheduler)
        {
            return Observable
                .Using(() => new MessageServiceContext(api, metaData), serviceContext =>
                {
                    var messagesInterval
                        = Observable.Interval(messageInterval, scheduler);

                    return messagesInterval
                            .TakeUntil(x => liveBroadcast.ActualEndTime.HasValue)
                            .Select(_ => api.LiveChatMessages.List(liveBroadcast.LiveChatId, "snippet,authorDetails"))
                            .Do(x => x.MaxResults = 50)
                            .Select(x => x.Execute())
                            .Retry(x => true,messageInterval)
                            .Delay(x => Observable.Return(TimeSpan.FromMilliseconds(x.PollingIntervalMillis ?? 0))) //TODO verwurschteln into interval
                            .Select(x => GetNewChatMessages(x.Items, serviceContext.CurrentMessages))
                            .Do(x => serviceContext.CurrentMessages.AddRange(x))
                            .Skip(1)
                            .SelectMany(x => x)
                    ;
                })
                .Publish()
                .RefCount();
        }

        private static IReadOnlyCollection<YoutubeChatMessage> GetNewChatMessages(IList<LiveChatMessage> chatMessage, IReadOnlyCollection<YoutubeChatMessage> existingMessages)
        {
            return chatMessage
                .Select(message => new YoutubeChatMessage(new ChatMessage(message.AuthorDetails.DisplayName, message.Snippet.TextMessageDetails.MessageText, YoutubeContext.Plattform), message.AuthorDetails.ChannelId, message.Snippet.PublishedAt.Value))
                .Except(existingMessages)
                .ToList();
        }


        private record FollowerServiceContext(YouTubeService Api, YoutubeMetaData MetaData) : IDisposable
        {
            public List<FollowInformation> CurrentFollowers { get; } = new();
            public Logger Logger { get; } = LogManager.GetLogger($"YoutubeFollower_{MetaData.User_Id}");

            public void Dispose()
            {
            }
        }
        private record MessageServiceContext(YouTubeService Api, YoutubeMetaData MetaData) : IDisposable
        {
            public List<YoutubeChatMessage> CurrentMessages { get; } = new();
            public Logger Logger { get; } = LogManager.GetLogger($"YoutubeMessages_{MetaData.User_Id}");

            public void Dispose()
            {
            }
        }

    }
}