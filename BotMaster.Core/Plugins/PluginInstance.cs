using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BotMaster.Core.Plugins
{
    public class PluginInstance : IDisposable, IEquatable<PluginServiceInstance>
    {
        public string Id => manifest.Id;

        public IObservable<Package> ReceivedPackages { get; private set; }

        private readonly PluginManifest manifest;
        private readonly Func<IObservable<Package>, IObservable<Package>> createServer;
        private readonly Subject<Package> sendPackages;
        private readonly IDisposable disposables;

        public PluginInstance(PluginManifest manifest, Func<IObservable<Package>, IObservable<Package>> createServer)
        {
            sendPackages = new Subject<Package>();

            this.manifest = manifest;
            this.createServer = createServer;

            disposables = StableCompositeDisposable.Create(sendPackages);
        }

        public virtual void Start()
        {
            ReceivedPackages = createServer(sendPackages)
                                .Publish()
                                .RefCount();
        }

        public IObservable<Package> Send(IObservable<Package> packages)
            => packages
                   .Do(p => sendPackages.OnNext(p));

        public void Dispose()
        {
            disposables.Dispose();
        }

        public override bool Equals(object obj)
            => Equals(obj as PluginServiceInstance);

        public bool Equals(PluginServiceInstance other)
            => other != null
               && manifest.Id == other.manifest.Id;

        public override int GetHashCode()
            => HashCode.Combine(manifest.Id);

        public static bool operator ==(PluginInstance left, PluginInstance right)
            => EqualityComparer<PluginInstance>.Default.Equals(left, right);
        public static bool operator !=(PluginInstance left, PluginInstance right)
            => !(left == right);
    }
}