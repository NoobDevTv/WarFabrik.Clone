using Microsoft.EntityFrameworkCore;

namespace BotMaster.Database.SqLite;
public class PostgresConfigurator : IDatabaseConfigurator
{
    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        _ = optionsBuilder.UseNpgsql(connectionString);
    }
}
