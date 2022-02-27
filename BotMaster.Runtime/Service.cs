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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Wrong Usage", "DF0020:Marks undisposed objects assinged to a field, originated in an object creation.", Justification = "<Ausstehend>")]
        public Service(ITypeContainer typeContainer, ILogger logger, DirectoryInfo pluginFolder, FileInfo pluginHost)
        {
            messageHub = new MessageHub();

            var packages =
                PluginProvider
                    .Watch(typeContainer, pluginFolder, pluginHost);

            pluginService = new PluginService(messageHub, packages);
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
