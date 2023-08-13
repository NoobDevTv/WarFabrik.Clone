using NLog;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Collections.Concurrent;
using BotMaster.PluginSystem.Connection;
using BotMaster.Core;
using System.Reactive.Subjects;

namespace BotMaster.PluginSystem
{
    public sealed class PluginService : IDisposable
    {
        public IReadOnlyDictionary<Guid, PluginInstance> Plugins => plugins;
        public Subject<PluginInstance> ChangedPlugins { get; }

        private readonly Logger logger;
        private readonly ConcurrentDictionary<Guid, PluginInstance> plugins;
        private readonly MessageHub messageHub;
        private readonly IObservable<(PluginManifest manifest, Guid instanceId)> manifests;
        private readonly IObservable<PluginConnection> connections;
        private readonly DirectoryInfo runnersPath;
        private IDisposable subscription;

        public PluginService(MessageHub messageHub,
            IObservable<(PluginManifest manifest, Guid instanceId)> manifests,
            IObservable<PluginConnection> connections,
            DirectoryInfo runnersPath,
            ServerRunnerService serverRunnerService)
        {
            logger = LogManager.GetLogger(nameof(PluginService));
            plugins = new();
            ChangedPlugins = new Subject<PluginInstance>();
            this.messageHub = messageHub;
            this.manifests = manifests;
            this.connections = connections;
            this.runnersPath = runnersPath;
            serverRunnerService.OnNewMessage += NewRunnerServiceMessage;
        }

        private void NewRunnerServiceMessage(Guid instanceId, System.Text.Json.JsonElement data)
        {
            if (!plugins.TryGetValue(instanceId, out var pluginInstance))
            {
                var error = $"God a message for an unknowon plugin id {instanceId}";
                logger.Error(error);
                throw new NotSupportedException(error);
            }

            if (data.TryGetProperty("PluginStatus", out var value))
            {
                var status = value.GetBoolean();
                if (!status && pluginInstance.Connection is not null)
                {
                    //TODO Disposing somehow inflicts damage upon other connections aswell
                    //pluginInstance.Connection.Dispose();
                    pluginInstance.Connection = null;
                    ChangedPlugins.OnNext(pluginInstance);
                }
            }
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
            ChangedPlugins.OnNext(pluginInstance);
        }

        private void OnNewManifest((PluginManifest manifest, Guid instanceId) registration)
        {
            logger.Info($"Got a new manifest {registration.manifest.Name}");

            if (!plugins.TryGetValue(registration.instanceId, out var instance))
            {
                instance = new ServerPluginInstance(registration.manifest, registration.instanceId, runnersPath);
                logger.Info($"Manifest {registration.manifest.Name} was turned into instance {instance.Id}");
                plugins[instance.Id] = instance;
                instance.Execute(Command.Recreate);
                ChangedPlugins.OnNext(instance);
            }
            else
            {
                instance.Manifest = registration.manifest;
                ChangedPlugins.OnNext(instance);
            }
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
