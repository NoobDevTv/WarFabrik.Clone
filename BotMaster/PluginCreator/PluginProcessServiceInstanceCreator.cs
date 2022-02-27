using BotMaster.PluginHost;
using BotMaster.PluginSystem;

namespace BotMaster.PluginCreator;
public class PluginServiceInstanceCreator : IPluginInstanceCreator
{
    public PluginInstance Create(
      PluginManifest manifest, FileInfo pluginHost, Func<IObservable<Package>, IObservable<Package>> createServer)
    {
        var logger = NLog.LogManager.GetLogger(manifest.Name);
        var paths = new FileInfo[] { manifest.CurrentFileInfo };

        var packages = PluginHoster.LoadAll(logger, paths);

        return new PluginServiceInstance(manifest, createServer, packages);
    }
}