using BotMaster.Core.Extensibility;
using BotMaster.PluginSystem.Messages;

using NLog;

using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace BotMaster.PluginSystem.PluginCreator
{
    public class TCPPluginCreator : IPluginInstanceCreator
    {
        private Dictionary<ushort, TcpListener> listeners = new();
        private ILogger ilogger;

        public TCPPluginCreator()
        {
            ilogger = NLog.LogManager.GetCurrentClassLogger();
        }

        public PluginInstance CreateClient(PluginManifest manifest)
        {
            var tcpData = manifest.ExtensionData["TcpData"];

            var port = tcpData.GetProperty("Port");
            ilogger.Debug($"Got Port {port}");
            var host = tcpData.GetProperty("Hostname");
            ilogger.Debug($"Got Hostname {host}");
            TcpClient a = new(host.GetString(), port.GetInt16());
            ilogger.Debug($"Connecting with tcp client");
            return new TCPPluginInstance(manifest, a);
        }

        public PluginInstance CreateServer(PluginManifest manifest, DirectoryInfo runnersPath)
        {
            var tcpData = manifest.ExtensionData["TcpData"];

            var port = tcpData.GetProperty("Port").GetUInt16();
            if (!listeners.ContainsKey(port))
            {
                var listener = new TcpListener(IPAddress.IPv6Any, port);
                ilogger.Debug($"Listening on Port {port}");
                listener.Server.DualMode = true;
                listener.Start();
                listeners.Add(port, listener);
            }
            return new TCPPluginInstance(runnersPath, manifest, () => GetClient(port));
        }

        private async Task<TcpClient> GetClient(ushort port)
        {
            TcpClient client = null;
            await Task.Run(() =>
            {
                client = listeners[port].AcceptTcpClient();
            });

            return client;
        }

    }

    public class TCPPluginInstance : PluginInstance
    {
        private readonly Process runnerProcess;
        private readonly CompositeDisposable compositeDisposable;
        private readonly DirectoryInfo runnersPath;
        private readonly Func<Task<TcpClient>>? clientRetriever = null;
        private TcpClient client;
        private NetworkStream networkStream;

        public TCPPluginInstance(PluginManifest manifest, TcpClient client) : base(manifest)
        {
            runnerProcess = null;
            compositeDisposable = null;
            runnersPath = null;
            this.client = client;
            networkStream = client.GetStream();
        }

        public TCPPluginInstance(
            DirectoryInfo runnersPath,
            PluginManifest manifest,
            Func<Task<TcpClient>> clientRetriever)
            : base(manifest)
        {
            this.runnersPath = runnersPath;
            this.clientRetriever = clientRetriever;
            var runnerManifestPath = new FileInfo(Path.Combine(runnersPath.FullName, manifest.ProcessRunner, "runner.manifest.json"));

            if (!runnerManifestPath.Exists)
                return; //TODO Logging, we cant load this plugin without the runner :(

            using var str = runnerManifestPath.OpenRead();
            var runnerManifest = System.Text.Json.JsonSerializer.Deserialize<RunnerManifest>(str);

            var runnersProcessPath = runnerManifest.Filename["default"];
            foreach (var item in runnerManifest.Filename)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Create(item.Key)))
                    runnersProcessPath = item.Value;
            }

            var args = runnerManifest.Args;
            foreach (var item in runnerManifest.EnviromentVariable)
            {
                args = args.Replace($"{{{item.Key}}}", item.Value
                    .Replace("{manifestpath}", manifest.CurrentFileInfo.FullName)
                    .Replace("{runnerpath}", runnerManifestPath.Directory.FullName)
                    );
            }
            clientRetriever().ContinueWith(async (t) =>
           {
               client = await t;
               networkStream = client.GetStream();
           });

            runnerProcess = new()
            {
                StartInfo = new ProcessStartInfo(runnersProcessPath, args)
                {
                    WorkingDirectory = manifest.CurrentFileInfo.Directory.FullName,
                    UseShellExecute = true
                },
                EnableRaisingEvents = true
            };
            compositeDisposable = new CompositeDisposable();
        }

        public override IObservable<Package> Send(IObservable<Package> packages) => packages.Do(x =>
        {
            Span<byte> buffer = stackalloc byte[x.Length];
            networkStream.Write(x.AsSpan(buffer));
        });

        public override IObservable<Package> Receiv()
        {
            return PluginConnection.CreateReceiverPipe(() => networkStream, (_) => client is not null && client.Connected).RetryDelayed(TimeSpan.FromSeconds(1));
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

        public override void Start()
        {
            if (runnerProcess is not null)
                runnerProcess.Start();
        }

        internal override PluginInstance Copy()
        {
            if (clientRetriever is null)
                return new TCPPluginInstance(manifest, client);
            else
                return new TCPPluginInstance(runnersPath, manifest, clientRetriever);

        }
    }
}