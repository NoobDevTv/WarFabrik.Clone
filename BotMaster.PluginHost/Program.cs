using BotMaster.Core.Plugins;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using NonSucking.Framework.Extension.Activation;
using NonSucking.Framework.Extension.IoC;
using System.Reactive.Linq;
using System.Threading;
using System.Reactive.Disposables;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace BotMaster.PluginHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using var logManager = Disposable.Create(() => LogManager.Shutdown());
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
            try
            {
                logger.Info("Start plugin host");
                using var manualReset = new ManualResetEvent(false);
                IDisposable pluginSub = default;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-l")
                    {
                        logger.Info("Load Manifest");
                        
                        i++;                        
                        pluginSub = Load(args[i], () => manualReset.Set(), logger);
                    }
                }

                using var b = pluginSub;

                manualReset.WaitOne();

            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Fatal Crash in application");
                throw;
            }
        }

        private static IDisposable Load(string fullName, Action reset, ILogger logger)
        {
            logger.Debug("Load manifest from " + fullName);
            var manifestFileInfo = new FileInfo(fullName);
            var manifest = JsonSerializer.Deserialize<PluginManifest>(File.ReadAllText(fullName), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            FileInfo assemblyFileInfo;

            if (Path.IsPathFullyQualified(manifest.File))
                assemblyFileInfo = new(manifest.File);
            else
                assemblyFileInfo = new(Path.Combine(manifestFileInfo.Directory.FullName, manifest.File));

            logger.Info($"Load {assemblyFileInfo.FullName}");
            var pluginAssembly = Assembly.LoadFrom(assemblyFileInfo.FullName);

            logger.Trace("Get Typecontainer");
            var typecontainer = TypeContainer.Get<ITypeContainer>();

            logger.Trace("Create plugin instance");
            var pluginInstance = new PluginInstance(
                                                   manifest,
                                                   observer => PluginClient.Create(manifest.Id, observer));

            logger.Info("Start plugin");
            pluginInstance.Start();

            logger.Trace("Get Assembly types");
            var plugins = pluginAssembly
                                    .GetTypes()
                                    .Where(t => t.IsAssignableTo(typeof(Plugin)))
                                    .Select(t => t.GetActivationDelegate<Plugin>()())
                                    .Do(p => p.Register(typecontainer))
                                    .ToObservable()
                                    .SelectMany(p => p.Start(pluginInstance.ReceivedPackages));

            logger.Debug("Subscribe process");
            var sub = pluginInstance
                                        .Send(plugins)
                                        .Subscribe(p => { }, ex => reset(), reset);

            return StableCompositeDisposable.Create(sub, typecontainer, pluginInstance);
        }
    }
}
