using BotMaster.PluginSystem.Messages;

using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BotMaster.PluginSystem
{

    public class PluginServiceInstance : PluginInstance
    {
        private readonly CompositeDisposable compositeDisposable;
        private readonly IObservable<Package> packages;
        private IDisposable packageDisposable;

        public PluginServiceInstance(
            PluginManifest manifest, Func<IObservable<Package>, IObservable<Package>> createServer, IObservable<Package> packages)
            : base(manifest, createServer)
        {
            compositeDisposable = new CompositeDisposable();
            this.packages = packages;
        }


        public override void Start()
        {
            base.Start();
            packageDisposable = packages.Subscribe();
        }

        public override void Dispose()
        {
            compositeDisposable.Dispose();
            packageDisposable.Dispose();
            base.Dispose();
        }

        internal override void ReceiveMessages(Func<string, IObservable<Message>> subscribeAsReceiver)
        {
            var sendPackages = Send(MessageConvert.ToPackage(subscribeAsReceiver(Id)));
            compositeDisposable.Add(sendPackages.Subscribe());
        }

        internal override void SendMessages(Func<IObservable<Message>, IDisposable> subscribeAsSender)
        {
            var receivedMessages = MessageConvert.ToMessage(ReceivedPackages);
            compositeDisposable.Add(subscribeAsSender(receivedMessages));
        }
    }
}