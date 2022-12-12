﻿using BotMaster.Core.Configuration;
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

        static int Main(string[] args)
        {
            //Thread.Sleep(30000);
            var config = ConfigManager.GetConfiguration("appsettings.json", args);

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

                var porcessCreator = new TCPPluginCreator();
                using var manualReset = new ManualResetEvent(false);
                bool error = false;
                using var disp = PluginHoster.LoadAll(logger, porcessCreator, paths, false)
                    .Subscribe(p => { }, ex =>
                    {
                        error = true;
                        logger.Fatal(ex);
                        manualReset.Set();
                        throw ex;
                    }, () => manualReset.Set());

                manualReset.WaitOne();
                return error ? 1 : 0;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Fatal Crash in application");
                throw;
            }
        }
    }
}
