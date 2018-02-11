using CommandManagementSystem;
using Newtonsoft.Json;
using System.IO;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Events.Services.FollowerService;
using TwitchLib.Logging;
using TwitchLib.Models.Client;
using TwitchLib.Services;

namespace WarFabrik.Clone
{
    public class Bot
    {
        private TwitchClient client;
        private BotCommandManager manager;
        private ConsoleLogger logger;
        private TwitchAPI api;
        private FollowerService followerService;


        public Bot()
        {
            var tokenFile = JsonConvert.DeserializeObject<TokenFile>(File.ReadAllText(@".\Token.json"));
            manager = new BotCommandManager();

            logger = new ConsoleLogger();
            api = new TwitchAPI(tokenFile.ClientId, tokenFile.Token);
            followerService = new FollowerService(api);

            var credentials = new ConnectionCredentials(tokenFile.Name, tokenFile.OAuth);
            client = new TwitchClient(credentials, channel: "noobdevtv", logging: true, logger: logger);

            client.OnConnected += ClientOnConnected;
            client.OnDisconnected += ClientOnDisconnected;
            client.OnMessageReceived += ClientOnMessageReceived;

            followerService.OnNewFollowersDetected += FollowerServiceOnNewFollowersDetected;

        }

        private void ClientOnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {

        }

        public void Connect() => client.Connect();

        public void Disconnect() => client.Disconnect();

        internal void SendMessage(string v) => client.SendMessage(v);

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

            manager.DispatchAsync(command, new BotCommandArgs(this, api, e.ChatMessage));
        }

        private void ClientOnConnected(object sender, OnConnectedArgs e)
        {
            logger.Info("Connected to Twitch Channel {Channel}");
            client.SendMessage($"Bot is Online...");
        }

        public void FollowerServiceOnNewFollowersDetected(object sender, OnNewFollowersDetectedArgs e)
        {
            //Task.Run(() =>
            //{
            //    using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            //    {
            //        var font = new Font("Calibri", 12, FontStyle.Bold);

            //        graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, 200, 200));
            //        graphics.DrawString("Neuer Follower", font, new SolidBrush(Color.Red), new PointF(0, 0));

            //        Thread.Sleep(5000);

            //    }

            //});


        }

        private void ClientOnDisconnected(object sender, OnDisconnectedArgs e)
        {
            logger.Info("Bot disconnect");
            client.SendMessage("Ich gehe in den Standby bb");
        }
    }
}
