using BotMaster.PluginSystem.Messages;

using System.Diagnostics;
using System.IO.Pipes;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BotMaster.PluginSystem.PluginCreator
{
    public class IPCPluginCreator : IPluginInstanceCreator
    {
        private static PluginInstance Create(
            PluginManifest manifest,
            FileInfo pluginHost,
            Func<string, PipeStream> createPipe,
            Func<PipeStream, IObservable<Package>,
            IObservable<Package>> createSender,
            Func<PipeStream, IObservable<Package>> createReceiver)
        {
            return new IPCPluginInstance(pluginHost, manifest, createPipe, createSender, createReceiver);
        }

        public PluginInstance CreateClient(PluginManifest manifest)
        {
            return new PluginInstance<PipeStream>(
                        manifest,
                        PluginClient.CreateClient,
                        PluginConnection.CreateSendPipe,
                        PluginConnection.CreateReceiverPipe
                    );
        }

        public PluginInstance CreateServer(PluginManifest manifest, FileInfo pluginHost)
        {
            return Create(
                manifest,
                pluginHost,
                PluginServer.CreateServer,
                PluginConnection.CreateSendPipe,
                PluginConnection.CreateReceiverPipe
            );
        }
    }

    public class IPCPluginInstance : PluginInstance<PipeStream>
    {
        private readonly Process process;
        private readonly CompositeDisposable compositeDisposable;

        public IPCPluginInstance(
            FileInfo pluginHost,
            PluginManifest manifest,
            Func<string, PipeStream> createPipe,
            Func<PipeStream, IObservable<Package>, IObservable<Package>> createSender,
            Func<PipeStream, IObservable<Package>> createReceiver)
            : base(manifest, createPipe, createSender, createReceiver)
        {
            process = new()
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

        internal override void ReceiveMessages(Func<string, IObservable<Message>> subscribeAsReceiver)
        {
            var sendPackages = Send(MessageConvert.ToPackage(subscribeAsReceiver(Id)));
            compositeDisposable.Add(sendPackages.Subscribe());
        }

        internal override void SendMessages(Func<IObservable<Message>, IDisposable> subscribeAsSender)
        {
            var receivedMessages = MessageConvert.ToMessage(Receiv());
            compositeDisposable.Add(subscribeAsSender(receivedMessages));
        }
    }
}