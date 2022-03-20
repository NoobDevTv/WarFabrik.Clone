using BotMaster.MessageContract;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using BotMaster.Twitch.MessageContract;

using Newtonsoft.Json;

using System.Reactive.Linq;

using TwitchLib.Api.Helix.Models.Users.GetUserFollows;
using TwitchLib.Client.Models;

using WarFabrik.Clone;


using DefinedMessageContract = BotMaster.MessageContract.Contract;

namespace BotMaster.Twitch
{
    public class TwitchService : Plugin
    {
        private readonly IMessageContractInfo[] messageContracts;

        public TwitchService()
        {
            messageContracts = new[]
            {
                (IMessageContractInfo)Betterplace.MessageContract.BetterplaceMessageContractInfo.Create()
            };
        }

        public override IObservable<Package> Start(IObservable<Package> receivedPackages)
            => MessageConvert.ToPackage(Create(MessageConvert.ToMessage(receivedPackages)));


        private static IObservable<Message> Create(IObservable<Message> notifications)
        {
            var info = new FileInfo(Path.Combine(".", "additionalfiles", "Token.json"));

            if (!info.Directory.Exists)
                info.Directory.Create();

            var tokenFile = JsonConvert.DeserializeObject<TokenFile>(File.ReadAllText(info.FullName));

            TryGetAccessToken(new FileInfo(Path.Combine(".", "additionalfiles", "access.json")), out var accessToken);

            return Bot.Create(tokenFile, accessToken, "NoobDevTv", notifications);

        }

        public override IEnumerable<IMessageContractInfo> ConsumeContracts()
            => messageContracts;


        private static bool TryGetAccessToken(FileInfo info, out AccessToken accessToken)
        {
            accessToken = null;

            if (info.Exists)
            {
                accessToken = JsonConvert.DeserializeObject<AccessToken>(File.ReadAllText(info.FullName));

                if (!accessToken.IsExpired)
                    return true;
            }
            return false;
        }

        private static async Task<AccessToken> GetAccessToken(FileInfo info, TokenFile tokenFile)
        {
            if (TryGetAccessToken(info, out var token))
                return token;

            token = await CreateToken(tokenFile);
            await File.WriteAllTextAsync(info.FullName, JsonConvert.SerializeObject(token));
            return token;
        }

        private static async Task<AccessToken> CreateToken(TokenFile tokenFile)
        {
            using var client = new HttpClient();
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
