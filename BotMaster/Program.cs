using BotMaster.Runtime;

using NLog;
using NLog.Config;
using NLog.Targets;

using System.Reactive.Disposables;

namespace BotMaster
{
    internal class Program
    {
        internal static async Task Main(string[] args)
        {

            var config = new LoggingConfiguration();

            var info = new FileInfo(Path.Combine(".", "additionalfiles", "botmaster.log"));
            var pluginInfo = new DirectoryInfo(Path.Combine(".", "plugins"));
            var pluginHost = new FileInfo(Path.Combine(".", "pluginhost", "BotMaster.PluginHost.exe"));

            if (!info.Directory.Exists)
                info.Directory.Create();
            if (!pluginInfo.Exists)
                pluginInfo.Create();

            using var fileTarget = new FileTarget("botmaster.logfile") { FileName = info.FullName };
#if DEBUG
            using var colorTarget = new ColoredConsoleTarget("botmaster.logconsole");
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, colorTarget);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
#else
            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
#endif
            LogManager.Configuration = config;

            using var managerDispose = Disposable.Create(() => LogManager.Shutdown());
            var logger = LogManager.GetCurrentClassLogger();

            using var resetEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) => resetEvent.Set();

            var serviceLogger = LogManager.GetLogger($"{nameof(BotMaster)}.{nameof(Service)}");
            using var service = new Service(serviceLogger, pluginInfo, pluginHost);
            service.Start();

            resetEvent.WaitOne();
        }
    }
}
