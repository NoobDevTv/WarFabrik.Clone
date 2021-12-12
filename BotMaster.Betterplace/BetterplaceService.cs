using BotMaster.Betterplace.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Reactive.Linq;
using BotMaster.Core;
using BotMaster.PluginSystem;

namespace BotMaster.Betterplace
{
    public sealed class BetterplaceService : Plugin
    {
        public IObservable<Opinion> Opinions { get; private set; }
        private readonly BetterplaceClient client;

        public BetterplaceService()
        {
            client = new BetterplaceClient("30639");

            Opinions = client
                .GetOpinionsPage(TimeSpan.FromMinutes(1))
                .Retry()
                .SelectMany(p => p.Data)
                .Where(o => o.Created_at > DateTime.Now.Subtract(TimeSpan.FromMinutes(4)))
                .Publish()
                .RefCount();
        }

        public override IObservable<Package> Start(IObservable<Package> receivedPackages) => throw new NotImplementedException();

    }
}
