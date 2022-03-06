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
        private readonly CancellationTokenSource cancellationTokenSource;

        public PluginServiceInstance(
            PluginManifest manifest, Func<IObservable<Package>, IObservable<Package>> createServer, IObservable<Package> packages)
            : base(manifest, createServer)
        {
            compositeDisposable = new CompositeDisposable();
            cancellationTokenSource = new();
            this.packages = packages;
        }


        public override void Start()
        {
            base.Start();
            Task.Run(() => packageDisposable = packages.Subscribe(), cancellationTokenSource.Token);
        }

        public override void Dispose()
        {
            compositeDisposable.Dispose();
            packageDisposable.Dispose();
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
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