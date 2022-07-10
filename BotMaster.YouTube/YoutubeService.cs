using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Database;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using NLog;

using System.Reactive.Linq;

namespace BotMaster.YouTube
{
    public class YoutubeService : Plugin
    {
        private readonly IMessageContractInfo[] messageContracts;
        private ILogger logger;

        public YoutubeService()
        {
            messageContracts = new[]
            {
                (IMessageContractInfo)Betterplace.MessageContract.BetterplaceMessageContractInfo.Create()
            };
        }

        public override IObservable<Package> Start(ILogger logger, IObservable<Package> receivedPackages)
        {
            using (var ctx = new RightsDbContext())
                ctx.Database.Migrate();
            using (var ctx = new UserConnectionContext())
                ctx.Database.Migrate();

            this.logger = logger;

            return MessageConvert.ToPackage(Bot.Create(MessageConvert.ToMessage(receivedPackages)));

        }

        public override IEnumerable<IMessageContractInfo> ConsumeContracts()
            => messageContracts;
    }
}
