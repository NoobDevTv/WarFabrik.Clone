using NLog;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Reactive.Subjects;
using System.Reactive.Disposables;

namespace BotMaster.PluginSystem.Connection.TCP;

public enum TCPHandshakingType
{
    Register,
    Started
}
public class TCPHandshakingService : IDisposable, IHandshakingService
{
    private TcpListener listener;
    private ILogger ilogger;
    private readonly Subject<PluginConnection> instanceSubject = new();
    private readonly Subject<(PluginManifest manifest, Guid instanceId)> manifestSubject = new();
    private readonly BotmasterConfig botmasterConfig;
    private readonly IDisposable connectionProviderDisposable;
    private readonly IDisposable manifestProviderDisposable;
    private TcpClient client;

    /// <summary>
    /// For client use only
    /// </summary>
    public TCPHandshakingService(ConnectionProvider provider)
    {
        ilogger = NLog.LogManager.GetCurrentClassLogger();
        provider.RegisterAsHandshakingService("TCP", this);
    }

    public TCPHandshakingService(BotmasterConfig botmasterConfig, ConnectionProvider provider, ManifestProvider manifestProvider)
    {
        ilogger = NLog.LogManager.GetCurrentClassLogger();
        this.botmasterConfig = botmasterConfig;
        provider.RegisterAsHandshakingService("TCP", this);
        connectionProviderDisposable = provider.RegisterSubProvider(instanceSubject);
        manifestProviderDisposable = manifestProvider.RegisterSubProvider(manifestSubject);
    }

    public void Start()
    {
        var port = botmasterConfig.PortForPluginCreation;
        listener = new TcpListener(IPAddress.IPv6Any, port);
        ilogger.Debug($"Listening on Port {port}");
        listener.Server.DualMode = true;
        listener.Start();
        listener.BeginAcceptTcpClient(BeginAcceptClient, null);
    }


    private void BeginAcceptClient(IAsyncResult ar)
    {
        var client = listener.EndAcceptTcpClient(ar);
        listener.BeginAcceptTcpClient(BeginAcceptClient, null);

        var ns = client.GetStream();
        Span<byte> handshakingType = stackalloc byte[4];
        ns.ReadExactly(handshakingType);
        var type = (TCPHandshakingType)BitConverter.ToInt32(handshakingType);

        if (type == TCPHandshakingType.Register)
        {

            Span<byte> guidBytes = stackalloc byte[16];
            ns.ReadExactly(guidBytes);
            var guid = new Guid(guidBytes);

            Span<byte> lengthBytes = stackalloc byte[4];
            ns.ReadExactly(lengthBytes);
            var length = BitConverter.ToInt32(lengthBytes);

            if (length > ushort.MaxValue)
            {
                return;
            }
            Span<byte> bytes = stackalloc byte[length];
            ns.ReadExactly(bytes);

            var content = Encoding.UTF8.GetString(bytes);
            var manifest = System.Text.Json.JsonSerializer.Deserialize<PluginManifest>(content);
            manifestSubject.OnNext((manifest, guid));
            ns.WriteByte(12);
        }
        else
        {

            Span<byte> lengthBytes = stackalloc byte[4];
            ns.ReadExactly(lengthBytes);
            var length = BitConverter.ToInt32(lengthBytes);

            Span<byte> bytes = stackalloc byte[length];
            ns.ReadExactly(bytes);

            var manifestId = Encoding.UTF8.GetString(bytes);

            Span<byte> guidBytes = stackalloc byte[16];
            ns.ReadExactly(guidBytes);
            var instanceId = new Guid(guidBytes);

            var sender = new Subject<Package>();

            var recPipe = PluginConnectionUtils.CreateReceiverPipe(ns, (_) => client.Connected);
            var sendedPackages = PluginConnectionUtils.CreateSendPipe(ns, sender, (_) => client.Connected);

            var con = new PluginConnection(instanceId,
                                           manifestId,
                                           sender,
                                           recPipe,
                                           StableCompositeDisposable.Create(sendedPackages.Subscribe(), ns, client, sender));

            //TODO Read whatever we need, manifest already exists, create connection
            instanceSubject.OnNext(con);
        }
    }

    public PluginConnection StartAsClient(PluginManifest manifest, Guid instanceId)
    {

        var tcpData = manifest.Connection.ExtensionData;

        var host = tcpData["Hostname"];
        var port = tcpData["Port"];

        client = new(host.GetString(), port.GetInt16());

        var str = client.GetStream();
        str.Write(BitConverter.GetBytes((int)TCPHandshakingType.Started));
        str.Write(BitConverter.GetBytes(manifest.Id.Length));
        str.Write(Encoding.UTF8.GetBytes(manifest.Id));
        str.Write(instanceId.ToByteArray());

        var sender = new Subject<Package>();

        var recPipe = PluginConnectionUtils.CreateReceiverPipe(str, (_) => client.Connected);
        var sendedPackages = PluginConnectionUtils.CreateSendPipe(str, sender, (_) => client.Connected);
        return new PluginConnection(instanceId,
                                    manifest.Id,
                                    sender,
                                    recPipe,
                                    StableCompositeDisposable.Create(sendedPackages.Subscribe(), str, client, sender));
    }

    public Guid RegisterAsClient(PluginManifest manifest, Guid? fixedInstanceId)
    {
        var tcpData = manifest.Connection;

        var host = tcpData.ExtensionData["Hostname"]; //Null Ref in Web UI, hier weitermachen :)
        var handshakePort = tcpData.ExtensionData["HandshakePort"];
        var port = tcpData.ExtensionData["Port"];
        ilogger.Debug($"Got Port {port}");
        ilogger.Debug($"Got HandshakePort {handshakePort}");
        ilogger.Debug($"Got Hostname {host}");

        using TcpClient b = new(host.GetString(), handshakePort.GetInt16());
        ilogger.Debug($"Connected to handshake server");
        using var str = b.GetStream();

        var pluginId = fixedInstanceId ?? Guid.NewGuid();
        var strManifest = System.Text.Json.JsonSerializer.Serialize(manifest);
        str.Write(BitConverter.GetBytes((int)TCPHandshakingType.Register));
        str.Write(pluginId.ToByteArray());
        str.Write(BitConverter.GetBytes(strManifest.Length));
        str.Write(Encoding.UTF8.GetBytes(strManifest));

        ilogger.Debug($"Written handshake");
        str.ReadByte();
        ilogger.Debug($"Finished handshake");

        //Thread.Sleep(5000);

        return pluginId;
    }

    public void Dispose()
    {
        connectionProviderDisposable?.Dispose();
        manifestProviderDisposable?.Dispose();
    }
}


