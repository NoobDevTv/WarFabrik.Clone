using BotMaster.PluginHost;
using BotMaster.PluginSystem;

using NLog;
using NLog.Config;
using NLog.Targets;

using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BotMaster.PluginHostProcessRunnermanifest
{
    class Program
    {

        static void Main(string[] args)
        {
            using var logManager = Disposable.Create(LogManager.Shutdown);
            var config = new LoggingConfiguration();

            var info = new FileInfo(Path.Combine(".", "logs", $"pluginhost-{DateTime.Now:ddMMyyyy-HHmmss_fff}.log"));

            if (!info.Directory.Exists)
                info.Directory.Create();

            using var consoleTarget = new ColoredConsoleTarget("pluginhost.logconsole");
            using var fileTarget = new FileTarget("pluginhost.logfile") { FileName = info.FullName };

#if DEBUG
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
#else
            config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
#endif
            LogManager.Configuration = config;
            var logger = LogManager.GetCurrentClassLogger();
            var plugins = new List<Plugin>();
            try
            {
                List<FileInfo> paths = new();
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-l")
                    {
                        i++;
                        paths.Add(new FileInfo(args[i]));
                    }
                }

                using var manualReset = new ManualResetEvent(false);

                using var disp = PluginHoster.LoadAll(logger, paths).Subscribe(p => { }, ex => manualReset.Set(), () => manualReset.Set());

                manualReset.WaitOne();

            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Fatal Crash in application");
                throw;
            }
        }
    }
}
