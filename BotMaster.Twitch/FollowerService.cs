using BotMaster.Core.NLog;

using NLog;

using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUserFollows;

namespace WarFabrik.Clone
{
    public class FollowerService
    {
        public static IObservable<FollowInformation> GetFollowerUpdates(TwitchAPI api, string userId, TimeSpan period, IScheduler scheduler)
        {
            return Observable.Using(() => new FollowerServiceContext(api, userId), serviceContext =>
            {
                return Observable
                    .Interval(period, scheduler)
                    .Select(_ => Observable.FromAsync(() => api.Helix.Users.GetUsersFollowsAsync(toId: serviceContext.UserId)))
                    .Concat()
                    .OnError(serviceContext.Logger, nameof(TwitchLib.Api.Helix.Users.GetUsersFollowsAsync))
                    .Retry()
                    .Select(followerResponse => GetNewFollower(followerResponse.Follows, serviceContext.CurrentFollowers))
                    .Do(x => serviceContext.CurrentFollowers.AddRange(x))
                    .Skip(1)
                    .SelectMany(x => x);
            });
        }

        private static IReadOnlyCollection<FollowInformation> GetNewFollower(Follow[] follows, IReadOnlyCollection<FollowInformation> alreadyExistingFollower)
        {
            return follows
                .Select(follow => new FollowInformation(follow.FromUserName, follow.FromUserId, follow.FollowedAt))
                .Except(alreadyExistingFollower)
                .ToList();
        }

        private record FollowerServiceContext(TwitchAPI Api, string UserId) : IDisposable
        {
            public List<FollowInformation> CurrentFollowers { get; } = new();
            public Logger Logger { get; } = LogManager.GetLogger($"{nameof(FollowerService)}_{UserId}");

            public void Dispose()
            {

            }
        }
    }
}