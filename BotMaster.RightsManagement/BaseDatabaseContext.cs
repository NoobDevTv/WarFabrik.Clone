
using BotMaster.Core.Configuration;
using BotMaster.Database;

using Microsoft.EntityFrameworkCore;

namespace BotMaster.RightsManagement;

public class BaseDatabaseContext : DatabaseContext
{
    public BaseDatabaseContext()
    {
    }

    public BaseDatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = ConfigManager.GetConfiguration(Path.Combine("additionalfiles", "Rightsconfig.json")).GetSettings<RightConfiguration>();

        DatabaseFactory.Initialize(config.DatabasePluginName);
        foreach (var item in DatabaseFactory.DatabaseConfigurators)
        {
            item.OnConfiguring(optionsBuilder, config.ConnectionString);
        }
        //var info = new FileInfo(config.DbPath);
        ////var info = new FileInfo(Path.Combine("..", "..", "additionalfiles", "Rights.db"));
        //_ = optionsBuilder.UseSqlite($"Data Source={info.FullName}");
        base.OnConfiguring(optionsBuilder);
    }
}
