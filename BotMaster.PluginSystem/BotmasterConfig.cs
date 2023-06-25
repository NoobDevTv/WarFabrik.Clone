using BotMaster.Core.Configuration;

namespace BotMaster.PluginSystem;
public class BotmasterConfig : ISetting
{
    public string ConfigName => "Botmaster";

    public string PluginCreator { get; set; }
    public string RunnersPath { get; set; }
    public string PluginsPath { get; set; }
    public string RemotePluginsPath { get; set; }
    public ushort PortForPluginCreation { get; set; } = 6789;
}
