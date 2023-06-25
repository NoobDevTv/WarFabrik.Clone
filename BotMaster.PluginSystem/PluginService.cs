using NLog;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Collections.Concurrent;
using BotMaster.PluginSystem.Connection;

namespace BotMaster.PluginSystem
{
    public sealed class PluginService : IDisposable
    {
        public IReadOnlyDictionary<Guid, PluginInstance> Plugins => plugins;

        private readonly Logger logger;
        private readonly ConcurrentDictionary<Guid, PluginInstance> plugins;
        private readonly MessageHub messageHub;
        private readonly IObservable<(PluginManifest manifest, Guid instanceId)> manifests;
        private readonly IObservable<PluginConnection> connections;
        private readonly DirectoryInfo runnersPath;
        private IDisposable subscription;

        public PluginService(MessageHub messageHub, IObservable<(PluginManifest manifest, Guid instanceId)> manifests, IObservable<PluginConnection> connections, DirectoryInfo runnersPath)
        {
            logger = LogManager.GetLogger(nameof(PluginService));
            plugins = new();
            this.messageHub = messageHub;
            this.manifests = manifests;
            this.connections = connections;
            this.runnersPath = runnersPath;
        }

        public void Start()
        {
            logger.Info("Start Service");
            subscription = StableCompositeDisposable.Create(
                manifests.Subscribe(OnNewManifest),
                connections.Subscribe(OnNewConnection)
                );
        }

        private void OnNewConnection(PluginConnection connection)
        {
            if (!plugins.TryGetValue(connection.InstanceId, out var pluginInstance))
            {
                var error = $"Creating a connection for a non existing plugin instance is not supported. Tried getting instance with id {connection.InstanceId}";
                logger.Error(error);
                throw new NotSupportedException(error);
            }
            logger.Info($"Got a new connection for instance {connection.InstanceId}");
            pluginInstance.Connection = connection;
            connection.SendMessages(messageHub.SubscribeAsSender);
            connection.ReceiveMessages(messageHub.GetFiltered);
        }

        private void OnNewManifest((PluginManifest manifest, Guid instanceId) registration)
        {
            logger.Info($"Got a new manifest {registration.manifest.Name}");
            var serverPluginInstance = new ServerPluginInstance(registration.manifest, registration.instanceId, runnersPath);
            logger.Info($"Manifest {registration.manifest.Name} was turned into instance {serverPluginInstance.Id}");
            plugins[serverPluginInstance.Id] = serverPluginInstance;
            serverPluginInstance.Start();
        }

        public void Stop()
        {
            logger.Info("Stop Service");
            subscription?.Dispose();
        }

        public void Dispose()
        {
            Stop();
            subscription?.Dispose();
        }
    }
}
