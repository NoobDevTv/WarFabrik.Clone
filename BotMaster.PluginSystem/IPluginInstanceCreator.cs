namespace BotMaster.PluginSystem;

public interface IPluginInstanceCreator
{
    PluginInstance Create(PluginManifest manifest, FileInfo pluginHost, Func<IObservable<Package>, IObservable<Package>> createServer);
}