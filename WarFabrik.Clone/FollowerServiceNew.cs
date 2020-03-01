using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib;
using TwitchLib.Api;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace WarFabrik.Clone
{
    public class FollowerServiceNew : IDisposable
    {
        private readonly List<FollowInformation> currentFollowers;

        public event EventHandler<NewFollowerDetectedArgs> OnNewFollowersDetected;

        private readonly TwitchAPI api;
        private readonly int timerInterval;
        private readonly Logger logger;

        private bool firstCall;

        private readonly System.Timers.Timer timer;

        /// <summary>
        /// Creates a new instance for the follower service. StartService has to be calles seperately
        /// </summary>
        /// <param name="api">Instance of the twitch api</param>
        /// <param name="channel">Name of your channel</param>
        /// <param name="period">time interval to check for new followers in miliseconds</param>
        public FollowerServiceNew(TwitchAPI api, int period)
        {
            timer = new System.Timers.Timer();

            this.api = api;
            timerInterval = period;
            logger = LogManager.GetCurrentClassLogger();
            firstCall = true;
            currentFollowers = new List<FollowInformation>();
        }

        public void StartService(string userId, CancellationToken token)
        {
            timer.Elapsed += (s, e) =>
            {
                var task = Task.Run(async () => await Loop(userId, token));
                task.Wait();
            };
            timer.Interval = timerInterval;
            timer.Start();
        }

        private async Task<IEnumerable<FollowInformation>> GetFollowsAsync(string userId)
        {
            var followerResponse = await api.Helix.Users.GetUsersFollowsAsync(toId: userId);
            return followerResponse.Follows.Select(f => new FollowInformation(f.FromUserName, f.FromUserId, f.FollowedAt));
        }

        private async Task Loop(string userId, CancellationToken token)
        {
            IEnumerable<FollowInformation> followInformations;
            try
            {
                followInformations = await GetFollowsAsync(userId);
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.GetType().Name}: {ex.Message}");
                return;
            }

            var newFollowers = followInformations.Except(currentFollowers).ToList();

            if (newFollowers.Count() > 0)
            {
                currentFollowers.AddRange(newFollowers);

                if (!firstCall)
                    OnNewFollowersDetected?.Invoke(this, new NewFollowerDetectedArgs { NewFollowers = newFollowers });
            }

            if (firstCall)
                firstCall = false;
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Dispose();
        }

        public class NewFollowerDetectedArgs
        {
            public List<FollowInformation> NewFollowers { get; internal set; }
        }
    }


}
