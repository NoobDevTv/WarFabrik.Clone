using BotMaster.PluginSystem;
using BotMaster.PluginSystem.PluginCreator;
using BotMaster.Runtime;

using NLog;
using NLog.Config;
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
            var config = new LoggingConfiguration();

            var info = new FileInfo(Path.Combine(".", "additionalfiles", "botmaster.log"));
            var pluginInfo = new DirectoryInfo(Path.Combine(".", "plugins"));
            var runnersPath = new DirectoryInfo(Path.Combine(".", "runners"));

            if (!info.Directory.Exists)
                info.Directory.Create();
            if (!pluginInfo.Exists)
                pluginInfo.Create();

            //LogManager.Setup().LoadConfigurationFromFile();

            using var fileTarget = new FileTarget("botmaster.logfile") { FileName = info.FullName };
            using var colorTarget = new ColoredConsoleTarget("botmaster.logconsole");
#if DEBUG
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, colorTarget);
#else
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, colorTarget);
#endif
            LogManager.Configuration = config;

            using var IDisposablemanagerDispose = Disposable.Create(LogManager.Shutdown);
            var logger = LogManager.GetCurrentClassLogger();

            logger.Info("BotMaster started");

            using var resetEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) => resetEvent.Set();
            var typeContainer = TypeContainer.Get<ITypeContainer>();

#if DEBUG
            var creatorLogger = LogManager.GetLogger("InProcessPlugin");
            typeContainer.Register<IPluginInstanceCreator>(new ProcessPluginCreator(creatorLogger, PluginHost.PluginHoster.Load));
            //typeContainer.Register<IPluginInstanceCreator>(new IPCPluginCreator());
#else
            var creatorLogger = LogManager.GetLogger("InProcessPlugin");

            typeContainer.Register<IPluginInstanceCreator>(new IPCPluginCreator());
            //typeContainer.Register<IPluginInstanceCreator>(new ProcessPluginCreator(creatorLogger, PluginHost.PluginHoster.Load));

#endif

            var serviceLogger = LogManager.GetLogger($"{nameof(BotMaster)}.{nameof(Service)}");

            logger.Info("BotMaster PluginService Started");
            using var service = new Service(typeContainer, serviceLogger, pluginInfo, runnersPath);
            service.Start();

            resetEvent.WaitOne();
            logger.Info("BotMaster stopped");
        }
    }
}
