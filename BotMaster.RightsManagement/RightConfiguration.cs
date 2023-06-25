
using BotMaster.Core.Configuration;

namespace BotMaster.RightsManagement;
public class RightConfiguration : ISetting
{
    public string ConfigName => nameof(RightConfiguration);
    public string ConnectionString { get; set; }
    public string DatabasePluginName { get; set; }
    public RightConfiguration()
    {
        ConnectionString = "Data Source=../../additionalfiles/Rights.db";
        DatabasePluginName = "BotMaster.Database.Sqlite.dll";
    }
}
