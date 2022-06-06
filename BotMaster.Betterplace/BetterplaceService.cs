using BotMaster.Betterplace.MessageContract;
using BotMaster.Betterplace.Model;
using BotMaster.Core;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using NLog;
using System.Reactive.Linq;

namespace BotMaster.Betterplace
{
    public sealed class BetterplaceService : Plugin
    {
        public IObservable<Opinion> Opinions { get; private set; }

        public BetterplaceService()
        {
            Opinions
                = BetterplaceClient
                .GetOpinionsPage("30639", TimeSpan.FromMinutes(1))
                .Retry()
                .SelectMany(p => p.Data)
                .Where(o => o.Created_at > DateTime.Now.Subtract(TimeSpan.FromMinutes(4)))
                .Publish()
                .RefCount();
        }

        public override IObservable<Package> Start(ILogger logger, IObservable<Package> receivedPackages)
          => MessageConvert
                .ToPackage(
                    Contract
                        .ToMessages(
                            Opinions
                                .Select(x => new BetterplaceMessage(x.ToDonation()))
                        )
                );
    }
}
