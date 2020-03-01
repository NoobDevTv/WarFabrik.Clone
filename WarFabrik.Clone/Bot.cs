using CommandManagementSystem;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using static WarFabrik.Clone.FollowerServiceNew;

namespace WarFabrik.Clone
{
    public class Bot
    {
        public string ChannelId { get; private set; }

        public FollowerServiceNew FollowerService { get; private set; }

        public event EventHandler<string> OnRaid;

        internal BotCommandManager Manager;

        private JoinedChannel initialChannel;

        private readonly TwitchClient client;
        private readonly TwitchAPI api;
        private readonly Logger logger;
        private bool disconnectRequested;

        public Bot()
        {
            disconnectRequested = false;
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
            client = new TwitchClient();
            client.Initialize(credentials, channel: "NoobDevTv", autoReListenOnExceptions: true);

            client.OnConnected += ClientOnConnected;
            client.OnDisconnected += ClientOnDisconnected;
            client.OnMessageReceived += ClientOnMessageReceived;
            client.OnRaidNotification += ClientOnRaidNotification;

            FollowerService = new FollowerServiceNew(api, 10000);
            FollowerService.OnNewFollowersDetected += FollowerServiceOnNewFollowersDetected;
        }

        public void Connect()
            => client.Connect();

        public void Disconnect()
        {
            disconnectRequested = true;
            client.Disconnect();
        }

        public async Task Run(CancellationToken token)
        {
            var users = await GetUsersAsync("NoobDevTv");
            ChannelId = users.FirstOrDefault()?.Id; //TODO: Multiple Results????
            
            Connect();
            FollowerService.StartService(ChannelId, token);
        }

        public void FollowerServiceOnNewFollowersDetected(object sender, NewFollowerDetectedArgs e)
        {
            foreach (var item in e.NewFollowers)
            {
                logger.Info(item.UserName + " has followed.");

                client.SendMessage(initialChannel, $"{item.UserName} hat sich verklickt. Vielen lieben Dank dafür <3");
                Manager.Dispatch("hype", new BotCommandArgs(this, api, null));
            }
        }

        internal void SendMessage(string v)
            => client.SendMessage(initialChannel, v);

        private async Task<IEnumerable<User>> GetUsersAsync(params string[] logins)
        {
            var userResponse = await api.Helix.Users.GetUsersAsync(logins: logins.ToList());
            return userResponse.Users;
        }

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
            client.SendMessage(initialChannel, $"Bot is Online...");
        }

        private void ClientOnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            if (!disconnectRequested)
            {
                Connect();
                return;
            }

            logger.Info("Bot disconnect");
            client.SendMessage(initialChannel, "Ich gehe in den Standby bb");
        }

        private void ClientOnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            if (!int.TryParse(e.RaidNotification.MsgParamViewerCount, out int count))
                count = 0;

            var channel = e.RaidNotification.MsgParamDisplayName;

            client.SendMessage(initialChannel, $"{channel} bringt jede Menge Noobs mit, nämlich 1 bis {count}. Yippie");
            OnRaid?.Invoke(this, e.RaidNotification.SystemMsgParsed);
        }

        private void ClientOnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {

        }
    }
}
