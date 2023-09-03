using BotMaster.PluginSystem;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using BotMaster.BotSystem.MessageContract;
using BotMaster.Core;

namespace BotMaster.Runtime
{
    internal class BotSystemService : IDisposable
    {
        private bool disposedValue;
        private readonly IDisposable disposable;
        private readonly PluginService pluginService;
        private readonly ServerRunnerService serverRunnerService;

        public BotSystemService(MessageHub messageHub, PluginService pluginService, ServerRunnerService serverRunnerService)
        {
            var incommingMessages = messageHub.GetFiltered("System");

            var systemMessages
                = SystemContract
                    .ToDefineMessages(incommingMessages)
                    .VisitMany(
                        getList => getList.Select(GetList),
                        lists => Observable.Empty<SystemMessage>(), //never
                        command => command.Select(command => ExecuteCommand(command.InstanceId, command.Command)),
                        info => Observable.Empty<SystemMessage>(),
                        commandChanged => Observable.Empty<SystemMessage>()
                    )
                    .Where(x => x is not null)
                    .Select(x => x!);
            var changedPlugins = pluginService.ChangedPlugins
                .Select(x => (SystemMessage)new PluginInfo(
                    x.Id,
                    x.Manifest.Name ?? "",
                    x.Manifest.Description ?? "",
                    x.Manifest.Author ?? "",
                    x.Manifest.Version ?? "",
                    x.Connection is not null));

            systemMessages = Observable.Merge(systemMessages, changedPlugins);

            var outgouingMessage = SystemContract.ToMessages(systemMessages);

#pragma warning disable DF0001 // Marks undisposed anonymous objects from method invocations.
            disposable = StableCompositeDisposable.Create(messageHub.SubscribeAsSender(outgouingMessage));
            this.pluginService = pluginService;
            this.serverRunnerService = serverRunnerService;
#pragma warning restore DF0001 // Marks undisposed anonymous objects from method invocations.
        }

        private SystemMessage? ExecuteCommand(Guid pluginId, Command command)
        {
            serverRunnerService.Execute(command, pluginId);
            //if (pluginService.Plugins.TryGetValue(pluginId, out var instance))
            //{

            //    instance.Execute(command);
            //}
            //return null;
            return GetList(default);
        }

        private SystemMessage GetList(GetPlugins obj)
        {
            foreach (var item in pluginService.Plugins)
            {
                serverRunnerService.Execute(Command.GetState, item.Key);
            }

            var plugins = pluginService.Plugins
                .Select(pluginPair => new PluginInfo(
                    pluginPair.Value.Id,
                    pluginPair.Value.Manifest.Name ?? "",
                    pluginPair.Value.Manifest.Description ?? "",
                    pluginPair.Value.Manifest.Author ?? "",
                    pluginPair.Value.Manifest.Version ?? "",
                    pluginPair.Value.Connection is not null))
                .ToList();
            return new PluginList()
            {
                PluginInfos = plugins
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposable?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
