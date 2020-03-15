﻿using BotMaster.Runtime;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BotMaster
{
    internal class Program
    {
        internal static async Task Main(string[] args)
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
            var logger = LogManager.GetCurrentClassLogger();

            using var resetEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) => resetEvent.Set();
            await new Service().Run();
            resetEvent.WaitOne();
        }
    }
}
