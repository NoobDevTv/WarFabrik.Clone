using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core.Extensibility;

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
        var baseResult = base.LoadUnmanagedDll(unmanagedDllName);
        if(baseResult != IntPtr.Zero)
            return baseResult;
        string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}
