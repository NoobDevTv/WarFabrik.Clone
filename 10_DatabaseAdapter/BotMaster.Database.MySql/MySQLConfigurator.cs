using Microsoft.EntityFrameworkCore;

namespace BotMaster.Database.SqLite;
public class MySQLConfigurator : IDatabaseConfigurator
{
    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        _ = optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}
