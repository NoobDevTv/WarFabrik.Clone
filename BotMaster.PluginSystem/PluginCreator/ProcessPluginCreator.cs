namespace BotMaster.PluginSystem.PluginCreator;

//public class ProcessPluginCreator : IPluginInstanceCreator
//{
//    private readonly Func<ILogger, IPluginInstanceCreator, FileInfo, IObservable<Package>> pluginLoader;
//    private readonly ILogger logger;
//    private readonly Dictionary<string, (Subject<Package> server, Subject<Package> client)> clients;

//    public ProcessPluginCreator(ILogger logger, Func<ILogger, IPluginInstanceCreator, FileInfo, IObservable<Package>> pluginLoader)
//    {
//        this.pluginLoader = pluginLoader;
//        this.logger = logger;
//        clients = new();
//    }

//    private PluginConnection Create(
//            PluginManifest manifest,
//            Func<string, InProcessClient> createPipe,
//            Func<InProcessClient, IObservable<Package>, IObservable<Package>> createSender,
//            Func<InProcessClient, IObservable<Package>> createReceiver)
//    {
//        return new PluginServiceInstance(manifest, LoadPlugin, createPipe, createSender, createReceiver);
//    }

//    public PluginConnection CreateClient(PluginManifest manifest)
//    {
//        return new PluginInstance<InProcessClient>(
//                    manifest,
//                    id => GetOrCreateClient(id, false),
//                    InProcessClient.CreateSendStream,
//                    InProcessClient.CreateReceiverStream
//                );
//    }

//    public PluginConnection CreateServer(PluginManifest manifest, DirectoryInfo runnersPath, bool local)
//    {
//        return Create(
//            manifest,
//            id => GetOrCreateClient(id, true),
//            InProcessClient.CreateSendStream,
//            InProcessClient.CreateReceiverStream
//        );
//    }

//    private IObservable<Package> LoadPlugin(FileInfo manifest)
//        => pluginLoader(logger, this, manifest);

//    private InProcessClient GetOrCreateClient(string id, bool isServer)
//    {
//        lock (clients)
//        {
//            if (!clients.TryGetValue(id, out var dataCouple))
//            {
//                dataCouple = (new(), new());
//                clients.Add(id, dataCouple);
//            }


//            if (!isServer)
//                return new(id, dataCouple.server, dataCouple.client);
//            else
//                return new(id, dataCouple.client, dataCouple.server);
//        }
//    }
//}