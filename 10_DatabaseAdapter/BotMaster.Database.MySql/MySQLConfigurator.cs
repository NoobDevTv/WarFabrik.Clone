using Microsoft.EntityFrameworkCore;

namespace BotMaster.Database.MySql;
public class MySQLConfigurator : IDatabaseConfigurator
{
    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        _ = optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}
