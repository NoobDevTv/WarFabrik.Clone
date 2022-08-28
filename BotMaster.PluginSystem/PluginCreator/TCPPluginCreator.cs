using BotMaster.PluginSystem.Messages;

using System.Diagnostics;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;

namespace BotMaster.PluginSystem.PluginCreator
{
    public class TCPPluginCreator : IPluginInstanceCreator
    {

        //Remove
        private static PluginInstance Create(
            PluginManifest manifest,
            DirectoryInfo runnersPath,
            Func<string, TcpListener> createPipe,
            Func<TcpClient, IObservable<Package>,
            IObservable<Package>> createSender,
            Func<TcpClient, IObservable<Package>> createReceiver)
        {
            return new TCPPluginInstance(runnersPath, manifest, createPipe, createSender, createReceiver);
        }

        //Just create listener and connector
        public PluginInstance CreateClient(PluginManifest manifest)
        {
            TcpClient a = null;
            NetworkStream ns = a.GetStream();

            return new PluginInstance<TcpClient>(
                        manifest,
                        NamedPipePluginClient.CreateClient,
                        (s, p)=> PluginConnection.CreateSendPipe(s.GetStream(), p, (_)=>s.Connected),
                        (s)=>PluginConnection.CreateReceiverPipe(s.GetStream(), (_) => s.Connected)
                    );
        }

        public PluginInstance CreateServer(PluginManifest manifest, DirectoryInfo runnersPath)
        {
            return Create(
                manifest,
                runnersPath,
                NamedPipePluginServer.CreateServer,
                (s, p) => PluginConnection.CreateSendPipe(s.GetStream(), p, (_) => s.Connected),
                (s) => PluginConnection.CreateReceiverPipe(s.GetStream(), (_) => s.Connected)
            );
        }

    }

    //Implement PluginInstance yourself
    public class TCPPluginInstance : PluginInstance
    {
        private readonly Process process;
        private readonly CompositeDisposable compositeDisposable;
        private int retries = 0;
        private readonly DirectoryInfo runnersPath;

        public TCPPluginInstance(
            DirectoryInfo runnersPath,
            PluginManifest manifest,
            Func<string, TcpListener> createPipe,
            Func<TcpClient, IObservable<Package>, IObservable<Package>> createSender,
            Func<TcpClient, IObservable<Package>> createReceiver) 
            : base(manifest)
        {
            this.runnersPath = runnersPath;
            var runnerManifestPath = new FileInfo(Path.Combine(runnersPath.FullName, manifest.ProcessRunner, "runner.manifest.json"));

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
                args = args.Replace($"{{{item.Key}}}", item.Value
                    .Replace("{manifestpath}", manifest.CurrentFileInfo.FullName)
                    .Replace("{runnerpath}", runnerManifestPath.Directory.FullName)
                    );
            }

            process = new()
            {
                StartInfo = new ProcessStartInfo(processPath, args)
                {
                    WorkingDirectory = manifest.CurrentFileInfo.Directory.FullName,
                    UseShellExecute = true
                },
                EnableRaisingEvents = true
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
            if (process.ExitCode == 0 || process.ExitCode == -1073741510)
                return;
            retries++;
            Thread.Sleep((retries + 1) * 10); //TODO Has this any side consequences, that we didn't consider in the heat of the moment?
            TriggerOnError(new ProcessExitedException(process.ExitCode));
        }

        public void Dispose()
        {
            compositeDisposable.Dispose();
            process.Dispose();
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


        internal override PluginInstance Copy() => new TCPPluginInstance(runnersPath, manifest, createPipe, createSender, createReceiver);

        public override IObservable<Package> Send(IObservable<Package> packages)
        {
            throw new NotImplementedException();
        }

        public override IObservable<Package> Receiv()
        {
            throw new NotImplementedException();
        }
    }
}