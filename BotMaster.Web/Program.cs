using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace BotMaster.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new LoggingConfiguration();

            var info = new FileInfo(Path.Combine(".", "additionalfiles", "botmaster.log"));

            if (!info.Directory.Exists)
                info.Directory.Create();

#if DEBUG
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, new ColoredConsoleTarget("botmaster.logconsole"));
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("botmaster.logfile") { FileName = info.FullName });
#else
            config.AddRule(LogLevel.Info, LogLevel.Fatal, new FileTarget("botmaster.logfile") { FileName = info.FullName });
#endif
            LogManager.Configuration = config;



            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseStartup<Startup>()
                    .UseKestrel();
                });
    }
}
