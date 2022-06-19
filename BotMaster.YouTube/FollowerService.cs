using BotMaster.Core.NLog;
using BotMaster.Twitch.MessageContract;

using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using NLog;

using System.Reactive.Concurrency;
using System.Reactive.Linq;


namespace BotMaster.YouTube
{
    public class FollowerService
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
                .Select(_=> api.Subscriptions.List("subscriberSnippet,contentDetails,snippet"))
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

        private record FollowerServiceContext(YouTubeService Api, YoutubeMetaData MetaData) : IDisposable
        {
            public List<FollowInformation> CurrentFollowers { get; } = new();
            public Logger Logger { get; } = LogManager.GetLogger($"{nameof(FollowerService)}_{MetaData.User_Id}");

            public void Dispose()
            {
            }
        }
    }
}