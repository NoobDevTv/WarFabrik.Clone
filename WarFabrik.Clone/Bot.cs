using CommandManagementSystem;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
using static WarFabrik.Clone.FollowerServiceNew;

namespace WarFabrik.Clone
{
    public class Bot
    {
        public FollowerServiceNew FollowerService;

        internal BotCommandManager Manager;

        private TwitchClient client;
        private readonly ConsoleLogger logger;
        private readonly JoinedChannel initialChannel;

        private TwitchAPI api;

        public Bot()
        {
            var tokenFile = JsonConvert.DeserializeObject<TokenFile>(File.ReadAllText(@".\Token.json"));
            Manager = new BotCommandManager();
            logger = new ConsoleLogger();
            api = new TwitchAPI();
            api.Settings.ClientId = tokenFile.ClientId;
            api.Settings.AccessToken = tokenFile.Token;
            
            var credentials = new ConnectionCredentials(tokenFile.Name, tokenFile.OAuth);
            //client = new TwitchClient(credentials, channel: "NoobDevTv", logging: true, logger: logger);
            client = new TwitchClient();
            client.Initialize(credentials, channel: "NoobDevTv", autoReListenOnExceptions: true);

            initialChannel = client.JoinedChannels.FirstOrDefault();
            FollowerService = new FollowerServiceNew(api, initialChannel, 10000);

            client.OnConnected += ClientOnConnected;
            client.OnDisconnected += ClientOnDisconnected;
            client.OnMessageReceived += ClientOnMessageReceived;

            FollowerService.OnNewFollowersDetected += FollowerServiceOnNewFollowersDetected;
            FollowerService.StartService();
        }

        private void ClientOnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {

        }

        public void Connect() => client.Connect();

        public void Disconnect() => client.Disconnect();

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
            //logger.Info("Connected to Twitch Channel {Channel}");
            client.SendMessage(initialChannel, $"Bot is Online...");
        }

        public void FollowerServiceOnNewFollowersDetected(object sender, NewFollowerDetectedArgs e)
        {
            foreach (var item in e.NewFollowers)
            {
                var tempColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(item.User.DisplayName + " has followed and ");
                if (item.Notifications)
                    Console.WriteLine("wants to be notified");
                else
                    Console.WriteLine("doesn't like to be notified");
                Console.ForegroundColor = tempColor;
            }

            Task.Run(() =>
            {
                using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    var font = new Font("Calibri", 12, FontStyle.Bold);
                    graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, 200, 200));
                    graphics.DrawString("Neuer Follower", font, new SolidBrush(Color.Red), new PointF(0, 0));
                    Thread.Sleep(5000);
                }
            });
        }

        private void ClientOnDisconnected(object sender, OnDisconnectedArgs e)
        {
            //logger.Info("Bot disconnect");
            client.SendMessage(initialChannel, "Ich gehe in den Standby bb");
        }
    }
}
