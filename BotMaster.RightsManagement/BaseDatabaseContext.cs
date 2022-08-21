using BotMaster.Configuration;
using BotMaster.Database;

using Microsoft.EntityFrameworkCore;

namespace BotMaster.RightsManagement;

public class BaseDatabaseContext : DatabaseContext
{

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = ConfigManager.GetConfiguration(Path.Combine("additionalfiles", "Rightsconfig.json")).GetSettings<RightConfiguration>();

        var info = new FileInfo(config.DbPath);
        //var info = new FileInfo(Path.Combine("..", "..", "additionalfiles", "Rights.db"));
        _ = optionsBuilder.UseSqlite($"Data Source={info.FullName}");
        base.OnConfiguring(optionsBuilder);
    }
}
