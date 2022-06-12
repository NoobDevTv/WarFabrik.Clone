using BotMaster.Core.NLog;
using BotMaster.PluginSystem;

using NLog;

using NonSucking.Framework.Extension.Activation;
using NonSucking.Framework.Extension.IoC;

using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace BotMaster.PluginHost
{
    public static class PluginHoster
    {
        public static IObservable<Package> LoadAll(ILogger logger, IPluginInstanceCreator creator, IReadOnlyCollection<FileInfo> paths)
        {
            logger.Info("Start plugin host");

            return Observable.Merge(paths.Select(info =>
            {
                logger.Info("Load Manifest");
                return Load(logger, creator, info);
            }));
        }

        public static IObservable<Package> Load(ILogger logger, IPluginInstanceCreator creator, FileInfo manifestFileInfo)
        {
            logger.Debug("Load manifest from " + manifestFileInfo.FullName);
            var manifest = JsonSerializer.Deserialize<PluginManifest>(File.ReadAllText(manifestFileInfo.FullName), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            FileInfo assemblyFileInfo;

            if (Path.IsPathFullyQualified(manifest.File))
                assemblyFileInfo = new(manifest.File);
            else
                assemblyFileInfo = new(Path.Combine(manifestFileInfo.Directory.FullName, manifest.File));


            logger.Info($"Load {assemblyFileInfo.FullName}");
            var pluginContext = new AssemblyLoadContext(manifest.Name);
            var resolver = new ReaderLoadContext(manifest.Name, assemblyFileInfo.FullName);

            var pluginAssembly = resolver.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(assemblyFileInfo.Name)));
            //var pluginAssembly = pluginContext.LoadFromAssemblyPath(assemblyFileInfo.FullName);


            logger.Trace("Get Typecontainer");
            var typecontainer = new StandaloneTypeContainer();

            logger.Trace("Create plugin instance");
            var pluginInstance = creator.CreateClient(manifest);

            logger.Info("Start plugin");
            pluginInstance.Start();

            var types = pluginAssembly
                    .GetTypes()
                    .Where(t => t.IsAssignableTo(typeof(Plugin)))
                    .ToList();

            logger.Trace("Get Assembly types");
            var packages =
                pluginAssembly
                    .GetTypes()
                    .Where(t => t.IsAssignableTo(typeof(Plugin)))
                    .Select(t => t.GetActivationDelegate<Plugin>()())
                    .Do(p => p.Register(typecontainer))
                    .Select(plugin => plugin.Start(logger, pluginInstance.Receiv()))
                    .Merge();

            logger.Debug("Subscribe process");
            return Observable.Using(() => new PluginContext(typecontainer, pluginInstance), (c) =>
                                          c.PluginInstance
                                          .Send(packages)
                                          .Log(logger, "Plugin_" + manifest.Name, onError: LogLevel.Fatal, onErrorMessage: (ex) => ex.ToString())
                                          );

        }

        public class ReaderLoadContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver _resolver;

            public ReaderLoadContext(string name, string readerLocation) : base(name)
            {
                _resolver = new AssemblyDependencyResolver(readerLocation);
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                var existing = Default.Assemblies.FirstOrDefault(x => x.FullName == assemblyName.FullName);
                if (existing is not null)
                    return existing;

                string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

                if (assemblyPath != null)
                {

                    return LoadFromAssemblyPath(assemblyPath);
                }

                return null;
            }

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

                if (libraryPath != null)
                {
                    return LoadUnmanagedDllFromPath(libraryPath);
                }

                return IntPtr.Zero;
            }
        }

        private record PluginContext(ITypeContainer TypeContainer, PluginInstance PluginInstance) : IDisposable
        {
            private bool disposedValue;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        TypeContainer.Dispose();

                        if(PluginInstance is IDisposable disposableInstance)
                            disposableInstance.Dispose();
                    }

                    disposedValue = true;
                }
            }



            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
