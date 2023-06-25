using System.Reactive.Subjects;

namespace BotMaster.PluginSystem.Connection;
public class ConnectionProvider : IDisposable
{
    private readonly Subject<PluginConnection> internalStream;
    private readonly Dictionary<string, IHandshakingService> handshakingServices;

    /*
     Implement resolvment of Handshaking Services, mostly for client side
     */

    public ConnectionProvider()
    {
        internalStream = new Subject<PluginConnection>();
        handshakingServices = new();
    }

    public PluginConnection Connect(PluginManifest manifest)
    {
        var type = manifest.Connection.Type; 
        if (!handshakingServices.TryGetValue(type, out var value))
        {
            //Better Error
            throw new ArgumentException($"Type not found {type}");
        }
        var dockerPluginInstanceId = Environment.GetEnvironmentVariable("DockerPluginInstanceId");

        Guid? fixedId = default;
        if (Guid.TryParse(dockerPluginInstanceId, out var worked))
            fixedId = worked;

        var id = value.RegisterAsClient(manifest, fixedId);
        return value.StartAsClient(manifest, id);
    }

    public void Start()
    {
        foreach (var item in handshakingServices)
        {
            item.Value.Start();
        }
    }

    public IObservable<PluginConnection> GetStream()
    {
        return internalStream;
    }

    public IDisposable RegisterSubProvider(IObservable<PluginConnection> provider)
    {
        return provider.Subscribe(internalStream);
    }

    public void RegisterAsHandshakingService(string key, IHandshakingService service) => handshakingServices[key] = service;

    public void Dispose()
    {
        internalStream.Dispose();
    }
}
