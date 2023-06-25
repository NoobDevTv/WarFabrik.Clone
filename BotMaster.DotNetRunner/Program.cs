using BotMaster.Core.Configuration;
using BotMaster.PluginHost;

using NLog;
using NLog.Extensions.Logging;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using BotMaster.PluginSystem.Connection;
using BotMaster.PluginSystem.Connection.TCP;

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

            logger.Debug("Gotten the following args: " + string.Join(" | ", args));


            try
            {
                FileInfo fi = default;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-l")
                    {
                        i++;
                        fi = new FileInfo(args[i]);
                    }
                }
                
                var processCreator = new ConnectionProvider();
                var tcp = new TCPHandshakingService(processCreator);
                _ = await PluginHoster.Load(logger, processCreator, fi, false)
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
