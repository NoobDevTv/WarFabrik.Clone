using BotMaster.Core.Configuration;
using BotMaster.PluginSystem;
using BotMaster.Runtime;

using NLog;
using NLog.Extensions.Logging;

using NonSucking.Framework.Extension.IoC;

using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BotMaster
{
    internal partial class Program
    {
        internal static async Task Main(string[] args)
        {
            var config = ConfigManager.GetConfiguration("appsettings.json", args);

            var botmasterConfig = config.GetSettings<BotmasterConfig>();

            using var iDisposablemanagerDispose = Disposable.Create(LogManager.Shutdown);

            var logger = LogManager
                .Setup()
                .LoadConfigurationFromSection(config)
                .GetCurrentClassLogger();

            logger.Info("BotMaster started");

            using var resetEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) => resetEvent.Set();
            var typeContainer = TypeContainer.Get<ITypeContainer>();

           

            typeContainer.Register(botmasterConfig);

            var serviceLogger = LogManager.GetLogger($"{nameof(BotMaster)}.{nameof(Service)}");

            logger.Info("BotMaster PluginService Started");
            using var service = new Service(typeContainer, serviceLogger);
            service.Start();

            resetEvent.WaitOne();
            logger.Info("BotMaster stopped");
        }
    }
}
