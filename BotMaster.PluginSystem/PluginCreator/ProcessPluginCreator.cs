
using NLog;

namespace BotMaster.PluginSystem.PluginCreator;

public class ProcessPluginCreator : IPluginInstanceCreator
{
    private readonly Func<ILogger, IPluginInstanceCreator, FileInfo, IObservable<Package>> pluginLoader;
    private readonly ILogger logger;

    public ProcessPluginCreator(ILogger logger, Func<ILogger, IPluginInstanceCreator, FileInfo, IObservable<Package>> pluginLoader)
    {
        this.pluginLoader = pluginLoader;
        this.logger = logger;
    }

    private PluginInstance Create(
            PluginManifest manifest,
            Func<string, InProcessClient> createPipe,
            Func<InProcessClient, IObservable<Package>, IObservable<Package>> createSender,
            Func<InProcessClient, IObservable<Package>> createReceiver)
    {
        return new PluginServiceInstance(manifest, LoadPlugin, createPipe, createSender, createReceiver);
    }

    public PluginInstance CreateClient(PluginManifest manifest)
    {
        return new PluginInstance<InProcessClient>(
                    manifest,
                    InProcessClient.Create,
                    InProcessClient.CreateSendStream,
                    InProcessClient.CreateReceiverStream
                );
    }

    public PluginInstance CreateServer(PluginManifest manifest, FileInfo pluginHost)
    {
        return Create(
            manifest,
            InProcessClient.Create,
            InProcessClient.CreateSendStream,
            InProcessClient.CreateReceiverStream
        );
    }

    private IObservable<Package> LoadPlugin(FileInfo manifest)
        => pluginLoader(logger, this, manifest);
}