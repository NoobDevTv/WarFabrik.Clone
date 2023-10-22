using BotMaster.Core.Extensibility;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Connection;

using NLog;

using NonSucking.Framework.Extension.Activation;
using NonSucking.Framework.Extension.IoC;

using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BotMaster.PluginHost
{

    public static class PluginHoster
    {
        private static ILogger iLogger;

        public static IObservable<Package> Load(ILogger logger, ConnectionProvider provider, FileInfo manifestFileInfo, bool loadIntoDifferentContext)
        {
            logger.Debug("Load manifest from " + manifestFileInfo.FullName);
            var manifest = JsonSerializer.Deserialize<PluginManifest>(File.ReadAllText(manifestFileInfo.FullName), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            iLogger = logger;

            FileInfo assemblyFileInfo;
            logger.Debug("Trying to load" + manifest.File);
            if (Path.IsPathRooted(manifest.File))
                assemblyFileInfo = new(manifest.File);
            else
                assemblyFileInfo = new(Path.Combine(manifestFileInfo.Directory.FullName, manifest.File));


            logger.Info($"Load {assemblyFileInfo.FullName}");
            AssemblyLoadContext resolver;
            if (loadIntoDifferentContext)
                resolver = new ReaderLoadContext(manifest.Name, assemblyFileInfo.FullName);
            else
                resolver = AssemblyLoadContext.Default;

            var _resolver = new AssemblyDependencyResolver(assemblyFileInfo.FullName);
            resolver.Resolving += (AssemblyLoadContext context, AssemblyName name) => Resolver_Resolving(context, name, _resolver);
            //if (resolver is not ReaderLoadContext)
            resolver.ResolvingUnmanagedDll += (Assembly ass, string name1) => Resolver_ResolvingUnmanagedDll(ass, name1, resolver, _resolver);

            var pluginAssembly = resolver.LoadFromAssemblyPath(assemblyFileInfo.FullName);

            //var pluginContext = new AssemblyLoadContext(manifest.Name);
            //var pluginAssembly = pluginContext.LoadFromAssemblyPath(assemblyFileInfo.FullName);

            logger.Trace("Get Typecontainer");
            var typecontainer = new StandaloneTypeContainer();

            logger.Trace("Create plugin instance");
            PluginConnection pluginInstance = provider.Connect(manifest);


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
                    .Select(plugin => plugin.Start(logger, pluginInstance.Receive()))
                    .Merge();

            logger.Debug("Subscribe process");

            return Observable
                .Using(
                    () => new PluginContext(typecontainer, pluginInstance, pluginInstance.Send(packages)),
                    (c) => Observable.Never<Package>()//c.PluginInstance.Receive()                                          
                );

        }

        private static Assembly Resolver_Resolving(AssemblyLoadContext context, AssemblyName name, AssemblyDependencyResolver _resolver)
        {
            if (name.Name.EndsWith("resources"))
                return null;

            var existing = context.Assemblies.FirstOrDefault(x => x.FullName == name.FullName);
            if (existing is not null)
                return existing;
            string assemblyPath = _resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                try
                {
                    var loaded = context.LoadFromAssemblyPath(assemblyPath);
                    if (loaded is not null)
                        return loaded;

                }
                catch (Exception)
                {
                    var bytes = File.ReadAllBytes(assemblyPath);
                    try
                    {
                        using var ms = new MemoryStream(bytes);
                        context.LoadFromStream(ms);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            assemblyPath = new FileInfo($"{name.Name}.dll").FullName;

            return context.LoadFromAssemblyPath(assemblyPath);
        }

        private static IntPtr Resolver_ResolvingUnmanagedDll(Assembly ass, string name, AssemblyLoadContext resolver, AssemblyDependencyResolver _resolver)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(name);

            if (libraryPath is null)
            {
                iLogger.Debug($"Trying to resolve from: {Directory.GetCurrentDirectory()} with name {name}");
                var identifier = RuntimeInformation.RuntimeIdentifier;
                identifier = Regex.Replace(identifier, @"(\w{3})[0-9]{1,2}(.+)", "$1$2");
                iLogger.Debug($"Trying to resolve from: {Directory.GetCurrentDirectory()} with {identifier}");
                if (!identifier.Contains("win"))
                    identifier = "linux-x64";
                var files = Directory.GetFiles(".", $"*{name}*", SearchOption.AllDirectories);
                iLogger.Debug($"Trying to resolve from: {Directory.GetCurrentDirectory()} found {files.Length}");

                libraryPath = files.FirstOrDefault(x => x.Contains(identifier, StringComparison.OrdinalIgnoreCase));
                if (libraryPath is not null)
                    libraryPath = Path.GetFullPath(libraryPath);
            }

            if (libraryPath is not null)
            {
                return (IntPtr)typeof(AssemblyLoadContext)
                    .GetMethod("LoadUnmanagedDllFromPath", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(resolver, new object[] { libraryPath });
                //return resolver.LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }


        private record PluginContext(ITypeContainer TypeContainer, PluginConnection PluginInstance, IDisposable AdditionalDisposable) : IDisposable
        {
            private bool disposedValue;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        TypeContainer.Dispose();

                        if (PluginInstance is IDisposable disposableInstance)
                            disposableInstance.Dispose();
                        AdditionalDisposable.Dispose();
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
