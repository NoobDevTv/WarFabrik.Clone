using BotMaster.PluginSystem;

using NLog;

using NonSucking.Framework.Extension.IoC;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System;
using BotMaster.PluginSystem.Messages;
using BotMaster.BotSystem.MessageContract;
using System.Reactive;

namespace BotMaster.Runtime
{
    internal class BotSystemService : IDisposable
    {
        private bool disposedValue;
        private readonly IDisposable disposable;
        private readonly PluginService pluginService;

        public BotSystemService(MessageHub messageHub, PluginService pluginService)
        {
            var incommingMessages = messageHub.GetFiltered("System");

            var systemMessages
                = SystemContract
                    .ToDefineMessages(incommingMessages)
                    .VisitMany(
                        getList => getList.Select(GetList),
                        lists => Observable.Empty<SystemMessage>(), //never
                        command => command.Select(command => ExecuteCommand(command.PluginId, command.Command))
                    )
                    .Where(x => x is not null)
                    .Select(x => x!);

            var outgouingMessage = SystemContract.ToMessages(systemMessages);

#pragma warning disable DF0001 // Marks undisposed anonymous objects from method invocations.
            disposable = StableCompositeDisposable.Create(messageHub.SubscribeAsSender(outgouingMessage));
            this.pluginService = pluginService;
#pragma warning restore DF0001 // Marks undisposed anonymous objects from method invocations.
        }

        private SystemMessage? ExecuteCommand(string pluginId, Command command)
        {
            if (pluginService.Plugins.TryGetValue(pluginId, out var instance))
            {
                switch (command)
                {
                    case Command.Start:
                        instance.Start();
                        break;
                    case Command.Stop:
                        instance.Stop();
                        break;
                }
                return GetList(default);
            }
            return null;
        }

        private SystemMessage GetList(GetPlugins obj)
        {
            var plugins = pluginService.Plugins
                .Select(pluginPair => new PluginInfo(
                    pluginPair.Value.Id,
                    pluginPair.Value.Manifest.Name ?? "",
                    pluginPair.Value.Manifest.Description ??"",
                    pluginPair.Value.Manifest.Author ?? "",
                    pluginPair.Value.Manifest.Version ?? "",
                    true))
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

    public class Service : IDisposable
    {
        private readonly PluginService pluginService;
        private readonly BotSystemService botSystemService;
        private readonly MessageHub messageHub;

        private readonly IDisposable disposable;
        private readonly ILogger logger;

        public Service(ITypeContainer typeContainer, ILogger logger)
        {
            messageHub = new MessageHub();

            var filePluginProvider = new FileManifestPluginProvider();
            var tcpPluginProvider = new TCPPluginProvider();

            var filePlugins =
                filePluginProvider
                    .GetPluginInstances(logger, typeContainer);

            var tcpPlugins = tcpPluginProvider.GetPluginInstances(logger, typeContainer);

            pluginService = new PluginService(messageHub, Observable.Merge(filePlugins, tcpPlugins));
            botSystemService = new BotSystemService(messageHub, pluginService);
            disposable = StableCompositeDisposable.Create(pluginService, messageHub);
            this.logger = logger;
        }

        public void Start()
        {
            pluginService.Start();
        }

        public void Stop()
        {
            pluginService.Stop();
        }

        public void Dispose()
        {
            Stop();
            disposable.Dispose();
            messageHub?.Dispose();
            pluginService?.Dispose();
            botSystemService?.Dispose();
        }
    }
}
