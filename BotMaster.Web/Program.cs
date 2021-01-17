using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BotMaster.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var logManager = Disposable.Create(() => LogManager.Shutdown());
            var config = new LoggingConfiguration();

            var info = new FileInfo(Path.Combine(".", "additionalfiles", "botmaster.log"));

            if (!info.Directory.Exists)
                info.Directory.Create();

            using var consoleTarget = new ColoredConsoleTarget("botmaster.logconsole");
            using var fileTarget = new FileTarget("botmaster.logfile") { FileName = info.FullName };

#if DEBUG
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, consoleTarget);
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, fileTarget);
#else
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, consoleTarget);
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, fileTarget);
#endif
            LogManager.Configuration = config;
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                using var host = CreateHostBuilder(args).Build();
                host.Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Fatal Crash in application");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseStartup<Startup>()
                    .UseKestrel();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}
