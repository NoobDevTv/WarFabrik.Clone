﻿using CommandManagementSystem;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using static WarFabrik.Clone.FollowerServiceNew;

namespace WarFabrik.Clone
{
    public class Bot
    {
        public string ChannelId { get; }

        public FollowerServiceNew FollowerService { get; private set; }

        public event EventHandler<(string Name, int Count)> OnHosted;

        internal BotCommandManager Manager;

        private TwitchClient client;
        private JoinedChannel initialChannel;


        private TwitchAPI api;
        private readonly Logger logger;

        public Bot()
        {
            logger = LogManager.GetCurrentClassLogger();
            var info = new FileInfo(Path.Combine(".", "additionalfiles", "Token.json"));

            if (!info.Directory.Exists)
                info.Directory.Create();

            var tokenFile = JsonConvert.DeserializeObject<TokenFile>(File.ReadAllText(info.FullName));
            Manager = new BotCommandManager();
            api = new TwitchAPI();
            api.Settings.ClientId = tokenFile.ClientId;
            api.Settings.AccessToken = tokenFile.Token;

            var credentials = new ConnectionCredentials(tokenFile.Name, tokenFile.OAuth);
            //client = new TwitchClient(credentials, channel: "NoobDevTv", logging: true, logger: logger);
            client = new TwitchClient();
            client.Initialize(credentials, channel: "NoobDevTv", autoReListenOnExceptions: true);

            bool initialId = true;
            do
            {
                try
                {
                    ChannelId = api.V5.Users.GetUserByNameAsync("NoobDevTv").Result.Matches.First().Id;
                    initialId = false;
                }
                catch (Exception ex)
                {
                    logger.Error($"{ex.GetType().Name}: {ex.Message}");
                    initialId = true;
                    Thread.Sleep(1000);
                }

            } while (initialId);

            FollowerService = new FollowerServiceNew(api, ChannelId, 10000);

            client.OnConnected += ClientOnConnected;
            client.OnDisconnected += ClientOnDisconnected;
            client.OnMessageReceived += ClientOnMessageReceived;
            client.OnBeingHosted += ClientOnBeingHosted;
            client.OnRaidNotification += ClientOnRaidNotification;

            FollowerService.OnNewFollowersDetected += FollowerServiceOnNewFollowersDetected;

        }
        
        public void Connect()
        {
            client.Connect();

        }

        public void Disconnect() => client.Disconnect();

        public void FollowerServiceOnNewFollowersDetected(object sender, NewFollowerDetectedArgs e)
        {
            foreach (var item in e.NewFollowers)
            {
                var msg = item.User.DisplayName + " has followed and ";

                if (item.Notifications)
                    msg += "wants to be notified";
                else
                    msg += "doesn't like to be notified";

                logger.Info(msg);

                client.SendMessage(initialChannel, $"{item.User.DisplayName} hat sich verklickt. Vielen lieben dank dafür <3");
                Manager.Dispatch("hype", new BotCommandArgs(this, api, null));
            }

        }

        internal void SendMessage(string v) => client.SendMessage(initialChannel, v);

        private void ClientOnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var message = e.ChatMessage.Message;

            if (!e.ChatMessage.Message.Contains("!") && !e.ChatMessage.IsMe)
                return;

            var index = message.IndexOf('!');
            var end = message.IndexOf(' ', index);

            if (end < 1)
                end = message.Length;

            var command = message.Substring(index, end - index).Trim().TrimStart('!').ToLower();

            Manager.DispatchAsync(command, new BotCommandArgs(this, api, e.ChatMessage));
        }

        private void ClientOnConnected(object sender, OnConnectedArgs e)
        {
            logger.Info($"Connected to Twitch Channel");
            initialChannel = client.JoinedChannels.FirstOrDefault();
            FollowerService.StartService();
            client.SendMessage(initialChannel, $"Bot is Online...");
        }

        private void ClientOnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            logger.Info("Bot disconnect");
            client.SendMessage(initialChannel, "Ich gehe in den Standby bb");
        }

        private void ClientOnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            if (!int.TryParse(e.RaidNotificaiton.MsgParamViewerCount, out int count))
                count = 0;
        }

        private void ClientOnBeingHosted(object sender, OnBeingHostedArgs e)
        {
            if (!e.BeingHostedNotification.IsAutoHosted)
                return;

            var channel = e.BeingHostedNotification.HostedByChannel;
            var count = e.BeingHostedNotification.Viewers;

            client.SendMessage(initialChannel, $"{channel} bring jede Menge Noobs mit, nämlich 1 bis {count}. Yippie");
            OnHosted?.Invoke(this, (channel, count));
        }

        private void ClientOnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {

        }
    }
}
