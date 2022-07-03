using BotMaster.PluginSystem.Messages;

using System.Diagnostics;
using System.IO.Pipes;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;

namespace BotMaster.PluginSystem.PluginCreator
{
    public class IPCPluginCreator : IPluginInstanceCreator
    {


        private static PluginInstance Create(
            PluginManifest manifest,
            DirectoryInfo runnersPath,
            Func<string, PipeStream> createPipe,
            Func<PipeStream, IObservable<Package>,
            IObservable<Package>> createSender,
            Func<PipeStream, IObservable<Package>> createReceiver)
        {
            return new IPCPluginInstance(runnersPath, manifest, createPipe, createSender, createReceiver);
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

        public PluginInstance CreateServer(PluginManifest manifest, DirectoryInfo runnersPath)
        {
            return Create(
                manifest,
                runnersPath,
                PluginServer.CreateServer,
                PluginConnection.CreateSendPipe,
                PluginConnection.CreateReceiverPipe
            );
        }
    }

    public class ProcessExitedException: Exception
    {
        public int ExitCode { get; set; }
        public ProcessExitedException(int exitCode)
        {
            ExitCode = exitCode;
        }
    }

    public class IPCPluginInstance : PluginInstance<PipeStream>
    {
        private readonly Process process;
        private readonly CompositeDisposable compositeDisposable;
        private int retries = 0;

        public IPCPluginInstance(
            DirectoryInfo runnersPath,
            PluginManifest manifest,
            Func<string, PipeStream> createPipe,
            Func<PipeStream, IObservable<Package>, IObservable<Package>> createSender,
            Func<PipeStream, IObservable<Package>> createReceiver)
            : base(manifest, createPipe, createSender, createReceiver)
        {
            var runnerManifestPath = new FileInfo( Path.Combine(runnersPath.FullName, manifest.ProcessRunner, "manifest.json"));

            if (!runnerManifestPath.Exists)
                return; //TODO Logging, we cant load this plugin without the runner :(

            using var str = runnerManifestPath.OpenRead();
            var runnerManifest = System.Text.Json.JsonSerializer.Deserialize<RunnerManifest>(str);
       
            var processPath = runnerManifest.Filename["default"];
            foreach (var item in runnerManifest.Filename)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Create(item.Key)))
                    processPath = item.Value;
            }

            var args = runnerManifest.Args;
            foreach (var item in runnerManifest.EnviromentVariable)
            {
                args = args.Replace($"{{{item.Key}}}", item.Value.Replace("{manifestpath}", manifest.CurrentFileInfo.FullName));
            }

            process = new()
            {
                StartInfo = new ProcessStartInfo(processPath, args)
                {
                    WorkingDirectory = manifest.CurrentFileInfo.Directory.FullName,
                    
                    UseShellExecute = true
                },
            };
            compositeDisposable = new CompositeDisposable();
        }

        internal void Kill()
        {
            process.Exited -= ProcessExited;
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
            process.Exited += ProcessExited;
            process.Start();
            process.Refresh();
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            process.Exited -= ProcessExited;
            if (process.ExitCode == 0)
                return;
            retries++;
            Thread.Sleep((retries + 1) * 10); //TODO Has this any side consequences, that we didn't consider in the heat of the moment?
            TriggerOnError(new ProcessExitedException(process.ExitCode));
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