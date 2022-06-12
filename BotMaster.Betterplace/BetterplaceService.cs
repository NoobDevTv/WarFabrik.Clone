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
        }

        public override IObservable<Package> Start(ILogger logger, IObservable<Package> receivedPackages)
        {
            if (Opinions is null)
                Opinions
                    = BetterplaceClient
                    .GetOpinionsPage("30639", TimeSpan.FromSeconds(20))
                    .Retry()
                    .SelectMany(p => p.Data)
                    .Where(o => o.Created_at > DateTime.Now.Subtract(TimeSpan.FromMinutes(400)))
                    .Publish()
                    .RefCount();

            return MessageConvert
                        .ToPackage(
                      Create(logger,
                          MessageConvert.ToMessage(receivedPackages)
                        ));
        }

        public IObservable<Message> Create(ILogger logger, IObservable<Message> receivedPackages)
        {
            return BetterplaceContract.ToMessages(Opinions.Select(x => new BetterplaceMessage(x.ToDonation())));
        }
    }
}
