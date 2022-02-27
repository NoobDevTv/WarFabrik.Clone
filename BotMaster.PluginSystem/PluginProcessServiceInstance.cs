using BotMaster.PluginSystem.Messages;

using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BotMaster.PluginSystem
{
    public class PluginProcessServiceInstanceCreator : IPluginInstanceCreator
    {
        public PluginInstance Create(
          PluginManifest manifest, FileInfo pluginHost, Func<IObservable<Package>, IObservable<Package>> createServer)
        {
            return new PluginProcessServiceInstance(manifest, pluginHost, createServer);
        }
    }

    public class PluginProcessServiceInstance : PluginInstance
    {
        private readonly Process process;
        private readonly CompositeDisposable compositeDisposable;

        public PluginProcessServiceInstance(
            PluginManifest manifest, FileInfo pluginHost, Func<IObservable<Package>, IObservable<Package>> createServer)
            : base(manifest, createServer)
        {
            this.process = new()
            {
                StartInfo = new(pluginHost.FullName, $"-l \"{manifest.CurrentFileInfo.FullName}\"")
                {
                    WorkingDirectory = manifest.CurrentFileInfo.Directory.FullName
                },
            };
            compositeDisposable = new CompositeDisposable();
        }

        internal void Kill()
        {
            process.Kill(true);
            process.WaitForExit(1000);
        }

        internal bool TryStop()
        {
            return process.WaitForExit(10000);
        }

        public override void Start()
        {
            base.Start();
            process.Start();
            process.Refresh();
        }

        public override void Dispose()
        {
            compositeDisposable.Dispose();
            process.Dispose();
            base.Dispose();
        }

        internal void ReceiveMessages(Func<string, IObservable<Message>> subscribeAsReceiver)
        {
            var sendPackages = Send(MessageConvert.ToPackage(subscribeAsReceiver(Id)));
            compositeDisposable.Add(sendPackages.Subscribe());
        }

        internal void SendMessages(Func<IObservable<Message>, IDisposable> subscribeAsSender)
        {
            var receivedMessages = MessageConvert.ToMessage(ReceivedPackages);
            compositeDisposable.Add(subscribeAsSender(receivedMessages));
        }
    }
}