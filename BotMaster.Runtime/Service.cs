using BotMaster.PluginSystem;

using NLog;

using NonSucking.Framework.Extension.IoC;

using System.Reactive.Disposables;

namespace BotMaster.Runtime
{
    public class Service : IDisposable
    {
        private readonly PluginService pluginService;
        private readonly MessageHub messageHub;

        private readonly IDisposable disposable;
        private readonly ILogger logger;

        public Service(ITypeContainer typeContainer, ILogger logger, DirectoryInfo pluginFolder, DirectoryInfo runnersPath)
        {
            messageHub = new MessageHub();

            var plugins =
                PluginProvider
                    .Watch(logger, typeContainer, pluginFolder, runnersPath);

            pluginService = new PluginService(messageHub, plugins);
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
