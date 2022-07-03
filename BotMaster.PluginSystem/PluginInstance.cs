using BotMaster.PluginSystem.Messages;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BotMaster.PluginSystem
{
    public abstract class PluginInstance
    {
        public string Id => manifest.Id;
        protected readonly PluginManifest manifest;

        internal event EventHandler<Exception> OnError;

        public PluginInstance(
            PluginManifest manifest)
        {
            this.manifest = manifest;
        }

        public virtual void Start()
        {
        }

        public abstract IObservable<Package> Send(IObservable<Package> packages);
        public abstract IObservable<Package> Receiv();

        internal virtual void SendMessages(Func<IObservable<Message>, IDisposable> subscribeAsSender) { }

        internal virtual void ReceiveMessages(Func<string, IObservable<Message>> subscribeAsReceiver) { }

        protected void TriggerOnError(Exception ex)
        {
            OnError?.Invoke(this, ex);
        }

        internal abstract PluginInstance Copy(); 

    }

    public class PluginInstance<TClient> : PluginInstance, IDisposable
    {
        protected TClient client;
        protected IObservable<Package> sendPipe;
        protected IObservable<Package> receivPipe;
        protected readonly Func<string, TClient> createPipe;
        protected readonly Func<TClient, IObservable<Package>, IObservable<Package>> createSender;
        protected readonly Func<TClient, IObservable<Package>> createReceiver;
        protected readonly Subject<Package> sendPackages;
        protected readonly CompositeDisposable disposables;

        public PluginInstance(
            PluginManifest manifest,
            Func<string, TClient> createPipe,
            Func<TClient, IObservable<Package>, IObservable<Package>> createSender,
            Func<TClient, IObservable<Package>> createReceiver) : base(manifest)
        {
            sendPackages = new Subject<Package>();

            this.createPipe = createPipe;
            this.createSender = createSender;
            this.createReceiver = createReceiver;

            disposables = new CompositeDisposable(sendPackages);
        }

        public override void Start()
        {
            base.Start();

            client = createPipe(Id);
            sendPipe = Observable.Defer(() => createSender(client, sendPackages)).Publish().RefCount();
            receivPipe = Observable.Defer(() => createReceiver(client)).Publish().RefCount();

            if (client is IDisposable disposableClient)
                disposables.Add(disposableClient);
        }

        public override IObservable<Package> Send(IObservable<Package> packages)
            => Observable.Using(() => packages.Subscribe(package => sendPackages.OnNext(package)), _ => sendPipe);

        public override IObservable<Package> Receiv()
            => receivPipe;

        public virtual void Dispose()
        {
            disposables.Dispose();
        }

        internal override PluginInstance Copy() => new PluginInstance<TClient>(manifest, createPipe, createSender, createReceiver);
    }
}