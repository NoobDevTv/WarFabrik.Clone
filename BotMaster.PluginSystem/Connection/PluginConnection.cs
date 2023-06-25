using BotMaster.Core.NLog;
using BotMaster.PluginSystem.Messages;

using NLog;

using System;

namespace BotMaster.PluginSystem.Connection
{

    public class ServerPluginInstance : PluginInstance, IDisposable
    {
        public RunnerInstance Runner { get; set; }
        public bool Running { get; set; }

        public ServerPluginInstance(PluginManifest manifest, Guid id, DirectoryInfo runnersPath) : base(manifest, id)
        {
            Runner = new RunnerInstance(runnersPath, manifest);
        }

        public override void Start()
        {
            Runner.Start();
        }

        public override void Stop()
        {
            if (!Runner.TryStop())
                Runner.Kill();
        }

        public void Dispose()
        {
            Runner?.Dispose();
        }
    }

    public class ClientPluginInstance : PluginInstance
    {
        public ClientPluginInstance(PluginManifest manifest, Guid id) : base(manifest, id)
        {
        }
    }

    public abstract class PluginInstance
    {
        public Guid Id { get; }
        public PluginManifest Manifest { get; private set; }

        public PluginConnection Connection { get; set; }

        public PluginInstance(PluginManifest manifest, Guid id)
        {
            Manifest = manifest;
            Id = id;
        }

        public virtual void Start()
        {
        }
        public virtual void Stop()
        {
        }
    }

    public class PluginConnection : IDisposable
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Guid InstanceId { get; }
        public string ManifestId { get; }
        public bool IsConnected { get; }
        public IDisposable sendDisposable { get; private set; }

        private readonly IObserver<Package> sendObserver;
        private readonly IObservable<Package> receiveObservable;
        private readonly IDisposable disposesOnClose;
        private readonly Logger logger;
        private IDisposable recDisposable;

        public PluginConnection(Guid instanceId,
            string manifestId,
            IObserver<Package> sendObservable,
            IObservable<Package> receiveObservable,
            IDisposable disposesOnClose)
        {
            InstanceId = instanceId;
            ManifestId = manifestId;
            this.sendObserver = sendObservable;
            this.receiveObservable = receiveObservable;
            this.disposesOnClose = disposesOnClose;
            logger = LogManager.GetCurrentClassLogger();
        }


        public IDisposable Send(IObservable<Package> packages)
        {
            //Clientside
            return packages.Log(logger, "Plugin_" + ManifestId, onError: LogLevel.Fatal, onErrorMessage: (ex) => ex.ToString()).Subscribe(sendObserver);
        }

        public IObservable<Package> Receive()
        {
            //Clientside
            return receiveObservable;
        }

        internal virtual void SendMessages(Func<IObservable<Message>, IDisposable> subscribeAsSender)
        {
            //Serverside
            sendDisposable = subscribeAsSender(MessageConvert.ToMessage(receiveObservable));
        }

        internal virtual void ReceiveMessages(Func<string, IObservable<Message>> subscribeAsReceiver)
        {
            //Serverside
            recDisposable = MessageConvert.ToPackage(subscribeAsReceiver(ManifestId)).Subscribe(sendObserver);
        }

        public void Dispose()
        {
            disposesOnClose.Dispose();
            recDisposable?.Dispose();
            sendDisposable?.Dispose();
        }
    }

    //public class PluginInstance<TClient> : PluginConnection, IDisposable
    //{
    //    protected TClient client;
    //    protected IObservable<Package> sendPipe;
    //    protected IObservable<Package> receivPipe;
    //    protected readonly Func<string, TClient> createPipe;
    //    protected readonly Func<TClient, IObservable<Package>, IObservable<Package>> createSender;
    //    protected readonly Func<TClient, IObservable<Package>> createReceiver;
    //    protected readonly Subject<Package> sendPackages;
    //    protected readonly CompositeDisposable disposables;
    //    public override bool IsConnected => false;

    //    public PluginInstance(
    //        PluginManifest manifest,
    //        Func<string, TClient> createPipe,
    //        Func<TClient, IObservable<Package>, IObservable<Package>> createSender,
    //        Func<TClient, IObservable<Package>> createReceiver) : base(manifest)
    //    {
    //        sendPackages = new Subject<Package>();

    //        this.createPipe = createPipe;
    //        this.createSender = createSender;
    //        this.createReceiver = createReceiver;

    //        disposables = new CompositeDisposable(sendPackages);
    //    }

    //    public override void Start()
    //    {
    //        base.Start();

    //        client = createPipe(Id);
    //        sendPipe = Observable.Defer(() => createSender(client, sendPackages)).Publish().RefCount();
    //        receivPipe = Observable.Defer(() => createReceiver(client)).Publish().RefCount();

    //        if (client is IDisposable disposableClient)
    //            disposables.Add(disposableClient);
    //    }

    //    public override IObservable<Package> Send(IObservable<Package> packages)
    //        => Observable.Using(() => packages.Subscribe(package => sendPackages.OnNext(package)), _ => sendPipe);

    //    public override IObservable<Package> Receive()
    //        => receivPipe;

    //    public virtual void Dispose()
    //    {
    //        disposables.Dispose();
    //    }

    //    internal override PluginConnection Copy() => new PluginInstance<TClient>(Manifest, createPipe, createSender, createReceiver);
    //}
}