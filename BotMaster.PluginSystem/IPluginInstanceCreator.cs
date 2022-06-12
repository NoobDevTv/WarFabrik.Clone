using System.IO.Pipes;

namespace BotMaster.PluginSystem;

public interface IPluginInstanceCreator
{
    PluginInstance CreateServer(
            PluginManifest manifest,
            FileInfo pluginHost);

    PluginInstance CreateClient(
            PluginManifest manifest);
}