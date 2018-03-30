using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;

namespace WarFabrik.Clone
{
    public class FollowerServiceNew
    {
        public List<IFollow> CurrentFollowers;

        public event EventHandler<NewFollowerDetectedArgs> OnNewFollowersDetected;

        private TwitchAPI api;
        private string channelName;
        private List<string> currentFollowerIds;
        private Timer timer;
        private int timerInterval;

        /// <summary>
        /// Creates a new instance for the follower service. StartService has to be calles seperately
        /// </summary>
        /// <param name="api">Instance of the twitch api</param>
        /// <param name="channel">Name of your channel</param>
        /// <param name="period">time interval to check for new followers in miliseconds</param>
        public FollowerServiceNew(TwitchAPI api, string channel, int period)
        {
            this.api = api;
            channelName = channel;
            timerInterval = period;
            timer = new Timer(async (o) => await Timer_Tick(o));
        }

        public void StartService()
        {
            var channel = api.Channels.v3.GetChannelByNameAsync(channelName).Result;
            var channelFollowers = api.Channels.v5.GetAllFollowersAsync(channel.Id).Result;

            CurrentFollowers = channelFollowers.Select(x=>(IFollow)x).ToList();
            currentFollowerIds = CurrentFollowers.Select(x => x.User.Id).ToList();

            timer.Change(0, timerInterval);
        }
        //TODO Warning not threadsafe

        private async Task Timer_Tick(object state)
        {
            var channel = api.Channels.v3.GetChannelByNameAsync(channelName).Result;
            var channelFollowers = await api.Channels.v5.GetChannelFollowersAsync(channel.Id, 100);
            var newFollowers = channelFollowers.Follows.Where(x => !currentFollowerIds.Contains(x.User.Id)).ToList();

            if (newFollowers.Count > 0)
            {
                CurrentFollowers.AddRange(newFollowers);
                currentFollowerIds.AddRange(newFollowers.Select(x => x.User.Id));
                OnNewFollowersDetected?.Invoke(this, new NewFollowerDetectedArgs { NewFollowers = newFollowers });
            }
        }
    }

    //TODO Export in own class?
    public class NewFollowerDetectedArgs
    {
        public List<IFollow> NewFollowers { get; internal set; }
    }
}
