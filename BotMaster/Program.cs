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
using System.Runtime.InteropServices;
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


            logger.Info("BotMaster started");

            using var resetEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) => resetEvent.Set();
            var typeContainer = TypeContainer.Get<ITypeContainer>();

            if (botmasterConfig.PluginCreator == nameof(ProcessPluginCreator))
            {
                var creatorLogger = LogManager.GetLogger("InProcessPlugin");
                typeContainer.Register<IPluginInstanceCreator>(new ProcessPluginCreator(creatorLogger, 
                    (l,pic,fi)=>PluginHost.PluginHoster.Load(l,pic,fi,true)));
            }
            else if (botmasterConfig.PluginCreator == nameof(NamedPipePluginCreator))
                typeContainer.Register<IPluginInstanceCreator>(new NamedPipePluginCreator());
            else if (botmasterConfig.PluginCreator == nameof(TCPPluginCreator))
            {
                var creator = new TCPPluginCreator();
                typeContainer.Register<IPluginInstanceCreator>(creator);
                typeContainer.Register(creator);

            }

            typeContainer.Register(botmasterConfig);

            var serviceLogger = LogManager.GetLogger($"{nameof(BotMaster)}.{nameof(Service)}");

            logger.Info("BotMaster PluginService Started");
            using var service = new Service(typeContainer, serviceLogger);
            service.Start();

            resetEvent.WaitOne();
            logger.Info("BotMaster stopped");
        }
    }
}
