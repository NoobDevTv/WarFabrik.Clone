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

namespace BotMaster.PluginHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using var manualReset = new ManualResetEvent(false);
            IDisposable pluginSub = default;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-l")
                {
                    i++;
                    pluginSub = Load(args[i], () => manualReset.Set());
                }
            }

            using var b = pluginSub;

            manualReset.WaitOne();
        }

        private static IDisposable Load(string fullName, Action reset)
        {
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

            var pluginAssembly = Assembly.LoadFrom(assemblyFileInfo.FullName);

            var typecontainer = TypeContainer.Get<ITypeContainer>();

            var pluginInstance = new PluginInstance(
                                                   manifest,
                                                   observer => PluginClient.Create(manifest.Id, observer));

            pluginInstance.Start();

            var plugins = pluginAssembly
                                    .GetTypes()
                                    .Where(t => t.IsAssignableTo(typeof(Plugin)))
                                    .Select(t => t.GetActivationDelegate<Plugin>()())
                                    .Do(p => p.Register(typecontainer))
                                    .ToObservable()
                                    .SelectMany(p => p.Start(pluginInstance.ReceivedPackages));

            var sub = pluginInstance
                                        .Send(plugins)
                                        .Subscribe(p => { }, ex => reset(), reset);

            return StableCompositeDisposable.Create(sub, typecontainer, pluginInstance);
        }
    }
}
