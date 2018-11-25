using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Api;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace WarFabrik.Clone
{
    public class FollowerServiceNew
    {
        public List<IFollow> CurrentFollowers;

        public event EventHandler<NewFollowerDetectedArgs> OnNewFollowersDetected;

        private TwitchAPI api;
        private List<string> currentFollowerIds;
        private Thread thread;
        private readonly int timerInterval;
        private readonly object lockObject;
        private bool runningThread;
        private readonly Logger logger;
        private Channel channel;

        public string ChannelId { get; }

        /// <summary>
        /// Creates a new instance for the follower service. StartService has to be calles seperately
        /// </summary>
        /// <param name="api">Instance of the twitch api</param>
        /// <param name="channel">Name of your channel</param>
        /// <param name="period">time interval to check for new followers in miliseconds</param>
        public FollowerServiceNew(TwitchAPI api, string channelId, int period)
        {
            this.api = api;
            timerInterval = period;
            ChannelId = channelId;
            runningThread = true;
            logger = LogManager.GetCurrentClassLogger();

            thread = new Thread(() =>
            {
                do
                {
                    Thread.Sleep(timerInterval);
                    TimerTick().Wait();

                } while (runningThread);
            });

            lockObject = new object();
            thread.Name = "ChannelListener";
            thread.IsBackground = true;
        }

        public void StartService()
        {
            bool initial = true;

            var channelFollowers = new List<ChannelFollow>();

            do
            {
                try
                {
                    channel = api.V5.Channels.GetChannelByIDAsync(ChannelId).Result;
                    Thread.Sleep(5000);
                    channelFollowers.AddRange(api.V5.Channels.GetAllFollowersAsync(channel.Id).Result);
                    initial = false;
                }
                catch (Exception ex)
                {
                    initial = true;
                    logger.Error($"{ex.GetType().Name}: {ex.Message}");
                    Thread.Sleep(1000);
                }

            } while (initial);

            lock (lockObject)
            {
                CurrentFollowers = channelFollowers.Select(x => (IFollow)x).ToList();
                currentFollowerIds = CurrentFollowers.Select(x => x.User.Id).ToList();
            }

            //thread.Change(0, timerInterval);
            thread.Start();
        }


        private async Task TimerTick()
        {
            ChannelFollowers channelFollowers;
            string id;

            lock (lockObject)
                id = channel.Id;


            try
            {
                channelFollowers = await api.V5.Channels.GetChannelFollowersAsync(id, 100);
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.GetType().Name}: {ex.Message}");
                Thread.Sleep(1000);
                return;
            }

            var newFollowers = channelFollowers.Follows.Where(x => !currentFollowerIds.Contains(x.User.Id)).ToList();

            if (newFollowers.Count > 0)
            {
                lock (lockObject)
                {
                    CurrentFollowers.AddRange(newFollowers);
                    currentFollowerIds.AddRange(newFollowers.Select(x => x.User.Id));
                }

                OnNewFollowersDetected?.Invoke(this, new NewFollowerDetectedArgs { NewFollowers = newFollowers });
            }
        }

        public class NewFollowerDetectedArgs
        {
            public List<IFollow> NewFollowers { get; internal set; }
        }
    }


}
