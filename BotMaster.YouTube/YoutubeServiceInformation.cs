using BotMaster.Core.NLog;
using BotMaster.MessageContract;
using BotMaster.Twitch.MessageContract;

using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using NLog;

using System.Reactive.Concurrency;
using System.Reactive.Linq;

using static Google.Apis.YouTube.v3.SearchResource.ListRequest;

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
                /*
                             var subsReq = youtubeService.Subscriptions.List("subscriberSnippet,contentDetails,snippet");
            subsReq.MyRecentSubscribers = true;

            subsReq.MaxResults = 10;
            var subsRes = subsReq.Execute();
            foreach (var item in subsRes.Items)
            {
                //item.Snippet.PublishedAt; When the follow occured
                //item.SubscriberSnippet.Title; Name of Follower

                Console.WriteLine($"{item.SubscriberSnippet.Title} follows since {item.Snippet.PublishedAt}");
            }
                 */
                return interval
                .Select(_ => api.Subscriptions.List("subscriberSnippet,contentDetails,snippet"))
                .Do(req => { req.MyRecentSubscribers = true; req.MaxResults = 10; })
                .Select(req => req.Execute())
                .OnError(serviceContext.Logger, nameof(api.Subscriptions))
                .Retry()
                .Select(followerResponse => GetNewFollower(followerResponse.Items, serviceContext.CurrentFollowers))
                .Do(x => serviceContext.CurrentFollowers.AddRange(x))
                .Skip(1)
                .SelectMany(x => x);
            });
        }

        private static IReadOnlyCollection<FollowInformation> GetNewFollower(IList<Subscription> follows, IReadOnlyCollection<FollowInformation> alreadyExistingFollower)
        {
            return follows
                .Select(follow => new FollowInformation(follow.SubscriberSnippet.Title, follow.SubscriberSnippet.Title, follow.Snippet.PublishedAt!.Value))
                .Except(alreadyExistingFollower)
                .ToList();
        }

        public static IObservable<LiveBroadcast?> GetBroadcasts(YouTubeService api, YoutubeMetaData metaData, TimeSpan liveStreamInterval, IScheduler scheduler)
        {

            var liveStreamsInterval
                = Observable.Return(0L).Concat(Observable.Interval(liveStreamInterval, scheduler));

            return liveStreamsInterval
                 .Select(_ => api.LiveBroadcasts.List("id,snippet,status"))
                 .Do(x => { x.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active; x.MaxResults = 1; })
                 .Select(x => x.Execute())
                 .DistinctUntilChanged(x => x.Items.FirstOrDefault()?.Snippet.LiveChatId)
                 .SelectMany(x => x.Items)
                 .Publish()
                 .RefCount()
                 ;
        }

        //public static IObservable<Video?> GetLiveVideo(YouTubeService api, YoutubeMetaData metaData, TimeSpan liveStreamInterval, IScheduler scheduler)
        //{
        //    //var liveStreamsInterval
        //    //    = Observable.Return(0L).Concat(Observable.Interval(liveStreamInterval, scheduler));

        //    return //liveStreamsInterval
        //           //.Select(_ => api.Search.List("snippet"))
        //           //.Do(x => { x.ChannelId = metaData.Channel_Id; x.Type = "video"; x.EventType = EventTypeEnum.Live; })
        //           //.Select(x => x.Execute().Items.FirstOrDefault())
        //           //.Where(x => x is not null) //No stream is currently running, should dispose serviceContext, but how?
        //           //.Select(x => (x, api.Videos.List("snippet,contentDetails,statistics,liveStreamingDetails")))
        //           //.Do(x => { x.Item2.Id = x.x.Id.VideoId; x.Item2.MaxResults = 1; x.Item2. })

        //          GetBroadcasts(api, metaData, liveStreamInterval, scheduler)
        //         .Where(x => x is not null) //No stream is currently running, should dispose serviceContext, but how?
        //         .Select(x => (x, api.Videos.List("snippet,contentDetails,statistics,liveStreamingDetails")))
        //         .Do(x => { x.Item2.Id = x.x.Id; x.Item2.MaxResults = 1; })
        //         .Select(x => x.Item2.Execute().Items.FirstOrDefault())//This should not be null, but we don't know
        //         .DistinctUntilChanged(x => x?.LiveStreamingDetails?.ActiveLiveChatId)
        //         .Publish()
        //         .RefCount()
        //         ;
        //}

        public static IObservable<YoutubeChatMessage> GetMessageUpdates(YouTubeService api, YoutubeMetaData metaData, TimeSpan messageInterval, LiveBroadcast liveBroadcast, IScheduler scheduler)
        {
            return Observable.Using(() => new MessageServiceContext(api, metaData), serviceContext =>
            {
                var messagesInterval
                    = Observable.Return(0L).Concat(Observable.Interval(messageInterval, scheduler));

                return messagesInterval
                        .Select(_ => api.LiveChatMessages.List(liveBroadcast.Snippet.LiveChatId, "snippet,authorDetails"))
                        .Do(x => x.MaxResults = 50)
                        .Select(x => x.Execute())
                        .Delay(x => Observable.Return(TimeSpan.FromMilliseconds(x.PollingIntervalMillis.HasValue ? x.PollingIntervalMillis.Value : 0)))
                        .Select(x => GetNewChatMessages(x.Items, serviceContext.CurrentMessages))
                        .Do(x => serviceContext.CurrentMessages.AddRange(x))
                        .Skip(1)
                        .SelectMany(x => x)
                        .Publish()
                        .RefCount()
                ;
            });
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