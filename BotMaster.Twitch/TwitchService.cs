using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using BotMaster.Telegram.Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog;
using System.Reactive.Linq;

namespace BotMaster.Twitch
{
    public class TwitchService : Plugin
    {
        private readonly IMessageContractInfo[] messageContracts;
        private ILogger logger;

        public TwitchService()
        {
            messageContracts = new[]
            {
                (IMessageContractInfo)Betterplace.MessageContract.BetterplaceMessageContractInfo.Create()
            };
        }

        public override IObservable<Package> Start(ILogger logger, IObservable<Package> receivedPackages)
        {
            using var ctx = new RightsDbContext();
            logger.Debug("Migrate RightsDbContext");
            ctx.Database.Migrate();

            this.logger = logger;

            return MessageConvert.ToPackage(Create(MessageConvert.ToMessage(receivedPackages.Do(package => logger.Trace("rcv new package")))));

        }


        private static IObservable<Message> Create(IObservable<Message> notifications)
        {
            var tokenFileInfo = new FileInfo(Path.Combine(".", "additionalfiles", "Token.json"));

            if (!tokenFileInfo.Directory.Exists)
                tokenFileInfo.Directory.Create();

            var tokenFile = JsonConvert.DeserializeObject<TokenFile>(File.ReadAllText(tokenFileInfo.FullName));

            return GetAccessToken(new FileInfo(Path.Combine(".", "additionalfiles", "access.json")), tokenFile)
                .Select(accessToken => Bot.Create(tokenFile, accessToken, "NoobDevTv", notifications))
                .Concat()
                .Retry();
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

        private static IObservable<AccessToken> GetAccessToken(FileInfo info, TokenFile tokenFile)
        {
            if (TryGetAccessToken(info, out var token))
                return Observable.Return(token);

            return CreateToken(tokenFile)
            .Do(token => File.WriteAllText(info.FullName, JsonConvert.SerializeObject(token)));
        }

        private static IObservable<AccessToken> CreateToken(TokenFile tokenFile)
        {
            var url = $"https://id.twitch.tv/oauth2/token?client_id={tokenFile.ClientId}&client_secret={tokenFile.ClientSecret}&grant_type=client_credentials";

            return Observable
                .Using(
                    () => new HttpClient(),
                    client =>
                        Observable
                        .Return(0)
                        .Select(_ => Observable.FromAsync(token => client.PostAsync(url, null, token)))
                        .Concat()
                        .Do(response => response.EnsureSuccessStatusCode())
                        .Retry()
                        .Select(response => Observable.FromAsync(token => response.Content.ReadAsStringAsync(token)))
                        .Concat()
                        .Select(rawToken => { var token = JsonConvert.DeserializeObject<AccessToken>(rawToken); token.CreatedAt = DateTime.Now; return token; })
                );
        }
    }
}
