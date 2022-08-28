using BotMaster.Core.Configuration;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.PluginCreator;
using BotMaster.Runtime;

using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
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
            var config = ConfigManager.GetConfiguration("appsettings.json", args);

            var botmasterConfig = config.GetSettings<BotmasterConfig>();


            using var iDisposablemanagerDispose = Disposable.Create(LogManager.Shutdown);

            var logger = LogManager
                .Setup()
                .LoadConfigurationFromSection(config)
                .GetCurrentClassLogger();

            var pluginInfo = new DirectoryInfo(botmasterConfig.PluginsPath);
            var runnersPath = new DirectoryInfo(botmasterConfig.RunnersPath);

            if (!pluginInfo.Exists)
                pluginInfo.Create();
            if (!runnersPath.Exists)
                runnersPath.Create();

            logger.Info("BotMaster started");

            using var resetEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) => resetEvent.Set();
            var typeContainer = TypeContainer.Get<ITypeContainer>();

            if (botmasterConfig.RunPluginsInOwnProcess)
            {
                var creatorLogger = LogManager.GetLogger("InProcessPlugin");
                typeContainer.Register<IPluginInstanceCreator>(new ProcessPluginCreator(creatorLogger, PluginHost.PluginHoster.Load));

            }
            else
            {
                typeContainer.Register<IPluginInstanceCreator>(new NamedPipePluginCreator());
            }

            var serviceLogger = LogManager.GetLogger($"{nameof(BotMaster)}.{nameof(Service)}");

            logger.Info("BotMaster PluginService Started");
            using var service = new Service(typeContainer, serviceLogger, pluginInfo, runnersPath);
            service.Start();

            resetEvent.WaitOne();
            logger.Info("BotMaster stopped");
        }
    }
}


//using System;

//// Token: 0x02000003 RID: 3
//public static class Program
//{
//    // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
//    public static void greet(string name)
//    {
//        Console.WriteLine("Morning, " + name + "!");
//    }

//    // Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
//    public static int Main()
//    {
//        Console.Write("Hello World 12");
//        Program.greet("Peter");
//        return 0;
//    }
//}
