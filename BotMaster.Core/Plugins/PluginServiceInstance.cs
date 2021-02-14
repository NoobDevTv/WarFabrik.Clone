using BotMaster.Core.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BotMaster.Core.Plugins
{
    public class PluginServiceInstance : PluginInstance
    {
        private readonly Process process;
        private readonly CompositeDisposable compositeDisposable;

        public PluginServiceInstance(
            PluginManifest manifest, Process process, Func<IObservable<Package>, IObservable<Package>> createServer)
            : base(manifest, createServer)
        {
            this.process = process;
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