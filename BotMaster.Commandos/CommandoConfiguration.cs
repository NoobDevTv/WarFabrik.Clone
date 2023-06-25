using BotMaster.Core.Configuration;

namespace BotMaster.Commandos;
public class CommandoConfiguration : ISetting
{
    public string ConfigName => nameof(CommandoConfiguration);
    public string ConnectionString { get; set; }
    public string DatabasePluginName { get; set; }

    public CommandoConfiguration()
    {
        ConnectionString = "Data Source=../../additionalfiles/Rights.db";
        DatabasePluginName = "bin\\Debug\\net7.0\\BotMaster.Database.SqLite.dll";
    }
}
