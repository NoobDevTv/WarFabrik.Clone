using Microsoft.EntityFrameworkCore;

namespace BotMaster.Database.SqLite;
public class SqLiteConfigurator : IDatabaseConfigurator
{
    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        _ = optionsBuilder.UseSqlite(connectionString);
    }
}
