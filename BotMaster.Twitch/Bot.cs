using Newtonsoft.Json;

using NLog;

using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
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

            Manager = new BotCommandManager();
            api = new TwitchAPI();

            client = new TwitchClient();

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
            var info = new FileInfo(Path.Combine(".", "additionalfiles", "Token.json"));

            if (!info.Directory.Exists)
                info.Directory.Create();

            var tokenFile = JsonConvert.DeserializeObject<TokenFile>(File.ReadAllText(info.FullName));

            var accessToken = await GetAccessToken(new FileInfo(Path.Combine(".", "additionalfiles", "access.json")), tokenFile);
            api.Settings.ClientId = tokenFile.ClientId;
            api.Settings.AccessToken = accessToken.Token;

            var credentials = new ConnectionCredentials(tokenFile.Name, tokenFile.OAuth);
            client.Initialize(credentials, channel: "NoobDevTv", autoReListenOnExceptions: true);


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
                Hype();
            }
        }

        public void Hype()
        {
            Manager.Dispatch("hype", new BotCommandArgs(this, api, null));
        }

        public void SendMessage(string message)
            => client.SendMessage(initialChannel, message);

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

            var command = message[index..end].Trim().TrimStart('!').ToLower();

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

        private static async Task<AccessToken> GetAccessToken(FileInfo info, TokenFile tokenFile)
        {
            AccessToken token;
            if (info.Exists)
            {
                token = JsonConvert.DeserializeObject<AccessToken>(await File.ReadAllTextAsync(info.FullName));

                if (!token.IsExpired)
                    return token;
            }

            token = await CreateToken(tokenFile);
            await File.WriteAllTextAsync(info.FullName, JsonConvert.SerializeObject(token));
            return token;
        }

        private static async Task<AccessToken> CreateToken(TokenFile tokenFile)
        {
            using var client = new HttpClient();
            using var content = new StringContent(string.Empty);
            var url = $"https://id.twitch.tv/oauth2/token?client_id={tokenFile.ClientId}&client_secret={tokenFile.ClientSecret}&grant_type=client_credentials";
            using var response = await client.PostAsync(url, null);

            using var status = response.EnsureSuccessStatusCode();

            var str = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<AccessToken>(str);
            token.CreatedAt = DateTime.Now;
            return token;
        }
    }
}
