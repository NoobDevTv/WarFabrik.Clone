using BotMaster.PluginSystem;

using NLog;

using NonSucking.Framework.Extension.IoC;

using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BotMaster.Runtime
{
    public class Service : IDisposable
    {
        private readonly PluginService pluginService;
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
        }
    }
}
