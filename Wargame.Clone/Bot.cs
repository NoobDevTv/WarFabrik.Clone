using CommandManagementSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Logging;
using TwitchLib.Models.Client;

namespace Wargame.Clone
{
    class Bot
    {
        private TwitchClient client;
        private DefaultCommandManager manager;
        private ConsoleLogger logger;


        public Bot()
        {
            var holder = new ManualResetEvent(false);

            var tokenFile = JsonConvert.DeserializeObject<TokenFile>(File.ReadAllText(@".\Token.json"));
            manager = new DefaultCommandManager("Wargame.Clone.Commands");

            logger = new ConsoleLogger();
            var api = new TwitchAPI(tokenFile.ClientId, tokenFile.Token);

            var credentials = new ConnectionCredentials(tokenFile.Name, tokenFile.OAuth);
            client = new TwitchClient(credentials, channel: "noobdevtv", logging: true, logger: logger);

            client.OnConnected += Client_OnConnected;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.Connect();

            holder.WaitOne();
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.Message.StartsWith("!") && !e.ChatMessage.IsMe)
                return;

            var strArray = e.ChatMessage.Message.Split();
            string[] args;

            if (strArray.Length > 1)
            {
                args = new string[strArray.Length - 1];
                strArray.CopyTo(args, 1);
            }
            else
            {
                args = new string[0];
            }

            manager.DispatchAsync(strArray[0].Trim().TrimStart('!').ToLower(), args);
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            logger.Info("Connected to Twitch Channel {Channel}");
            client.SendMessage($"Bot is Online...");
        }
    }
}
