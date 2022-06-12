using BotMaster.PluginSystem.Messages;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BotMaster.PluginSystem
{
    public class PluginServiceInstance : PluginInstance<InProcessClient>
    {
        private readonly CompositeDisposable compositeDisposable;
        private readonly Func<FileInfo, IObservable<Package>> loadPlugin;

        public PluginServiceInstance(
            PluginManifest manifest,
            Func<FileInfo, IObservable<Package>> pluginLoader,
            Func<string, InProcessClient> createPipe,
            Func<InProcessClient, IObservable<Package>, IObservable<Package>> createSender,
            Func<InProcessClient, IObservable<Package>> createReceiver)
            : base(manifest, createPipe, createSender, createReceiver)
        {
            compositeDisposable = new CompositeDisposable();
            loadPlugin = pluginLoader;
        }

        public override void Dispose()
        {
            compositeDisposable.Dispose();
            base.Dispose();
        }

        public override void Start()
        {
            base.Start();
            var subscription = loadPlugin(manifest.CurrentFileInfo).Subscribe();
            compositeDisposable.Add(subscription);
        }

        internal override void ReceiveMessages(Func<string, IObservable<Message>> subscribeAsReceiver)
        {
            var sendPackages = Send(MessageConvert.ToPackage(subscribeAsReceiver(Id)));
            var subscription = sendPackages.Subscribe();
            compositeDisposable.Add(subscription);
        }

        internal override void SendMessages(Func<IObservable<Message>, IDisposable> subscribeAsSender)
        {
            var receivedMessages = MessageConvert.ToMessage(Receiv());
            var subscription = subscribeAsSender(receivedMessages);
            compositeDisposable.Add(subscription);
        }
    }
}