using BotMaster.Configuraiton;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.PluginCreator;
using BotMaster.Runtime;

using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

using NonSucking.Framework.Extension.IoC;

using System.Diagnostics;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

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

            var pluginInfo = new DirectoryInfo(botmasterConfig.PluginsPath);
            var runnersPath = new DirectoryInfo(botmasterConfig.RunnersPath);

            if (!pluginInfo.Exists)
                pluginInfo.Create();
            if (!runnersPath.Exists)
                runnersPath.Create();

            logger.Info("BotMaster started");

            using var resetEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) => resetEvent.Set();
            var typeContainer = TypeContainer.Get<ITypeContainer>();

            if (botmasterConfig.RunPluginsInOwnProcess)
            {
                var creatorLogger = LogManager.GetLogger("InProcessPlugin");
                typeContainer.Register<IPluginInstanceCreator>(new ProcessPluginCreator(creatorLogger, PluginHost.PluginHoster.Load));

            }
            else
            {
                typeContainer.Register<IPluginInstanceCreator>(new IPCPluginCreator());
            }

            var serviceLogger = LogManager.GetLogger($"{nameof(BotMaster)}.{nameof(Service)}");

            logger.Info("BotMaster PluginService Started");
            using var service = new Service(typeContainer, serviceLogger, pluginInfo, runnersPath);
            service.Start();

            resetEvent.WaitOne();
            logger.Info("BotMaster stopped");
        }
    }
}
