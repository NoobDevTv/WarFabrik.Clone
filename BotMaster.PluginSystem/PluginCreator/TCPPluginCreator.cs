namespace BotMaster.PluginSystem.PluginCreator
{
    //public class TCPPluginCreator : IPluginInstanceCreator
    //{
    //    private Dictionary<ushort, TcpListener> listeners = new();
    //    private ILogger ilogger;

    //    public TCPPluginCreator()
    //    {
    //        ilogger = NLog.LogManager.GetCurrentClassLogger();
    //    }

    //    public PluginConnection CreateClient(PluginManifest manifest)
    //    {
    //        var tcpData = manifest.ExtensionData["TcpData"];

    //        var host = tcpData.GetProperty("Hostname");
    //        var handshakePort = tcpData.GetProperty("HandshakePort");
    //        var port = tcpData.GetProperty("Port");
    //        ilogger.Debug($"Got Port {port}");
    //        ilogger.Debug($"Got HandshakePort {handshakePort}");
    //        ilogger.Debug($"Got Hostname {host}");

    //        TcpClient b = new(host.GetString(), handshakePort.GetInt16());
    //        ilogger.Debug($"Connected to handshake server");
    //        var str = b.GetStream();

    //        var strManifest = System.Text.Json.JsonSerializer.Serialize(manifest);

    //        str.Write(BitConverter.GetBytes(strManifest.Length));
    //        str.Write(Encoding.UTF8.GetBytes(strManifest));


    //        ilogger.Debug($"Written handshake");
    //        Thread.Sleep(5000);

    //        TcpClient a = new(host.GetString(), port.GetInt16());
    //        ilogger.Debug($"Connecting with tcp client");
    //        a.SendTimeout = 1000;
    //        return new TCPPluginInstance(manifest, a);
    //    }

    //    public PluginConnection CreateServer(PluginManifest manifest, DirectoryInfo runnersPath, bool local)
    //    {
    //        var tcpData = manifest.ExtensionData["TcpData"];

    //        var port = tcpData.GetProperty("Port").GetUInt16();
    //        if (!listeners.ContainsKey(port))
    //        {
    //            var listener = new TcpListener(IPAddress.IPv6Any, port);
    //            ilogger.Debug($"Listening on Port {port}");
    //            listener.Server.DualMode = true;
    //            listener.Start();
    //            listeners.Add(port, listener);
    //        }

    //        Func<Task<TcpClient>>? receiver = !local ? () => GetClient(port) : null;

    //        return new TCPPluginInstance(runnersPath, manifest, receiver);
    //    }

    //    private async Task<TcpClient> GetClient(ushort port)
    //    {
    //        TcpClient client = null;
    //        await Task.Run(() =>
    //        {
    //            client = listeners[port].AcceptTcpClient();
    //            client.NoDelay = true;
    //            client.SendTimeout = 1000;
    //        });

    //        return client;
    //    }

    //}

    //public class TCPPluginInstance : PluginConnection, IDisposable
    //{
    //    public override bool IsConnected => client?.Connected ?? Manifest.ManifestInfo == ManifestInfo.Remote;

    //    private readonly Process runnerProcess;
    //    private readonly CompositeDisposable compositeDisposable;
    //    private readonly DirectoryInfo runnersPath;
    //    private readonly Func<Task<TcpClient>>? clientRetriever = null;
    //    private TcpClient client;
    //    private NetworkStream networkStream;
    //    private readonly Logger logger = LogManager.GetCurrentClassLogger();

    //    private static volatile int nextId = 0;

    //    private readonly int instanceId = nextId++;

    //    public TCPPluginInstance(PluginManifest manifest, TcpClient client) : base(manifest)
    //    {
    //        runnerProcess = null;
    //        compositeDisposable = new();
    //        runnersPath = null;
    //        this.client = client;
    //        networkStream = client.GetStream();
    //    }

    //    public TCPPluginInstance(
    //        DirectoryInfo runnersPath,
    //        PluginManifest manifest,
    //        Func<Task<TcpClient>> clientRetriever)
    //        : base(manifest)
    //    {
    //        this.clientRetriever = clientRetriever;
    //        this.runnersPath = runnersPath;

    //        if (this.runnersPath is not null)
    //        {
    //            var runnerManifestPath = new FileInfo(Path.Combine(runnersPath.FullName, manifest.ProcessRunner, "runner.manifest.json"));

    //            if (!runnerManifestPath.Exists)
    //                return; //TODO Logging, we cant load this plugin without the runner :(

    //            using var str = runnerManifestPath.OpenRead();
    //            var runnerManifest = System.Text.Json.JsonSerializer.Deserialize<RunnerManifest>(str);

    //            var runnersProcessPath = runnerManifest.Filename["default"];
    //            foreach (var item in runnerManifest.Filename)
    //            {
    //                if (RuntimeInformation.IsOSPlatform(OSPlatform.Create(item.Key)))
    //                    runnersProcessPath = item.Value;
    //            }

    //            var args = runnerManifest.Args;
    //            foreach (var item in runnerManifest.EnviromentVariable)
    //            {
    //                args = args.Replace($"{{{item.Key}}}", item.Value
    //                    .Replace("{manifestpath}", manifest.CurrentFileInfo.FullName)
    //                    .Replace("{runnerpath}", runnerManifestPath.Directory.FullName)
    //                    );
    //            }

    //            runnerProcess = new()
    //            {
    //                StartInfo = new ProcessStartInfo(runnersProcessPath, args)
    //                {
    //                    WorkingDirectory = manifest.CurrentFileInfo.Directory.FullName,
    //                    UseShellExecute = true
    //                },
    //                EnableRaisingEvents = true
    //            };
    //        }

    //        compositeDisposable = new CompositeDisposable();

    //        clientRetriever?.Invoke().ContinueWith(async (t) =>
    //        {
    //            client = await t;
    //            networkStream = client.GetStream();
    //            compositeDisposable.Add(client);
    //            compositeDisposable.Add(networkStream);
    //        });
    //    }

    //    public override IObservable<Package> Send(IObservable<Package> packages)
    //    {
    //        return PluginConnectionUtils.
    //            CreateSendPipe(() => networkStream, packages, (_) => networkStream is not null && client is not null && client.Connected)
    //            .Log(logger, "SendPipe", subscriptionMessage: () => "Created send pipe again", subscription: LogLevel.Info, onError: LogLevel.Error, onErrorMessage: (e) => e.ToString())
    //            .Retry(ex => ex is PluginConnectionException, TimeSpan.FromSeconds(1))
    //            ;
    //    }

    //    public override IObservable<Package> Receive()
    //    {

    //        return PluginConnectionUtils
    //            .CreateReceiverPipe(() => networkStream, (_) => networkStream is not null && client is not null && client.Connected)
    //            .Log(logger, "ReceivePipe", subscriptionMessage: () => "Created Receive pipe again", subscription: LogLevel.Info, onError: LogLevel.Error, onErrorMessage: (e) => $"Error happend in Instance Id {instanceId}:" + e.ToString())
    //            .Retry(ex => ex is PluginConnectionException, TimeSpan.FromSeconds(1))
    //            ;
    //    }

    //    internal override void ReceiveMessages(Func<string, IObservable<Message>> subscribeAsReceiver)
    //    {
    //        var sendPackages = Send(MessageConvert.ToPackage(subscribeAsReceiver(Id)));
    //        compositeDisposable.Add(sendPackages.Subscribe());
    //    }

    //    internal override void SendMessages(Func<IObservable<Message>, IDisposable> subscribeAsSender)
    //    {
    //        var receivedMessages = MessageConvert.ToMessage(Receive());
    //        compositeDisposable.Add(subscribeAsSender(receivedMessages));
    //    }

    //    public override void Start()
    //    {
    //        if (runnerProcess is not null)
    //            runnerProcess.Start();
    //    }

    //    public override void Stop()
    //    {
    //        if (runnerProcess is null)
    //            return;
    //        runnerProcess.StartInfo.ArgumentList.Add("-s");
    //        runnerProcess.Start();
    //    }

    //    internal override PluginConnection Copy()
    //    {
    //        if (clientRetriever is null)
    //            return new TCPPluginInstance(Manifest, client);
    //        else
    //            return new TCPPluginInstance(runnersPath, Manifest, clientRetriever);

    //    }

    //    public void Dispose()
    //    {
    //        client?.Close();
    //        compositeDisposable?.Dispose();
    //    }
    //}
}