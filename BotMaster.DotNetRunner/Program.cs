using BotMaster.Core.Configuration;
using BotMaster.PluginHost;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.PluginCreator;

using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Extensions.Logging;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Diagnostics;

namespace BotMaster.DotNetRunner
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var config = ConfigManager.GetConfiguration("appsettings.json", args);
            var debug = config.GetSection("Debug");
            var wait = debug.GetSection("Wait");
            if (!string.IsNullOrWhiteSpace(wait.Value) && int.TryParse(wait.Value, out var value))
            {
                Thread.Sleep(value);
            }

            using var logManager = Disposable.Create(LogManager.Shutdown);

            var info = new FileInfo(Path.Combine(".", "logs", $"pluginhost-{DateTime.Now:ddMMyyyy-HHmmss_fff}.log"));

            if (!info.Directory.Exists)
                info.Directory.Create();

            var logger = LogManager
                .Setup()
                .LoadConfigurationFromSection(config)
                .GetCurrentClassLogger();

            var plugins = new List<Plugin>();
            logger.Debug("Gotten the following args: " + string.Join(" | ", args));

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

                var processCreator = new TCPPluginCreator();

                _ = await PluginHoster.LoadAll(logger, processCreator, paths, false)
                    .IgnoreElements()
                    .Do(p => { }, ex =>
                        {
                            logger.Fatal(ex);
                            Environment.Exit(111);
                        })
                    ;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Fatal Crash in application");
                throw;
            }
        }
    }
}
