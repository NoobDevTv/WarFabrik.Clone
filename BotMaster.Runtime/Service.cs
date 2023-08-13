using BotMaster.PluginSystem;

using NLog;

using NonSucking.Framework.Extension.IoC;

using System.Reactive.Disposables;
using BotMaster.PluginSystem.Connection;
using BotMaster.PluginSystem.Connection.TCP;

namespace BotMaster.Runtime
{

    public class Service : IDisposable
    {
        private readonly PluginService pluginService;
        private readonly BotSystemService botSystemService;
        private readonly MessageHub messageHub;
        private readonly ConnectionProvider connectionProvider;
        private readonly IDisposable disposable;
        private readonly ILogger logger;
        private readonly ManifestProvider manifestProvider;
        private readonly ServerRunnerService serverRunnerService;

        public Service(ITypeContainer typeContainer, ILogger logger)
        {
            messageHub = new MessageHub();
            connectionProvider = new ConnectionProvider();
            typeContainer.Register(connectionProvider);
            manifestProvider = new ManifestProvider();
            typeContainer.Register(manifestProvider);
            serverRunnerService = new ServerRunnerService();
            typeContainer.Register(serverRunnerService);

            var bc = typeContainer.Get<BotmasterConfig>();
            var tcp = new TCPHandshakingService(bc, connectionProvider, manifestProvider);
            typeContainer.Register(tcp);

            var pluginInfo = new DirectoryInfo(bc.PluginsPath);

            if (!pluginInfo.Exists)
                pluginInfo.Create();
            var runnersPath = new DirectoryInfo(bc.RunnersPath);
            if (!pluginInfo.Exists)
                pluginInfo.Create();


            pluginService = new PluginService(messageHub, manifestProvider.GetStream(), connectionProvider.GetStream(), runnersPath, serverRunnerService);

            var fileManifestProvider = new FileManifestProvider(pluginInfo, manifestProvider);
            typeContainer.Register(fileManifestProvider);
            botSystemService = new BotSystemService(messageHub, pluginService, serverRunnerService);
            disposable = StableCompositeDisposable.Create(pluginService, messageHub);

            this.logger = logger;
        }

        public void Start()
        {
            connectionProvider.Start();
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
